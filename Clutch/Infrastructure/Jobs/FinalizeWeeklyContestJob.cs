using Clutch.Database;
using Clutch.Database.Entities.Clips.Extensions;
using Clutch.Database.Entities.Contests;
using Clutch.Database.Entities.ContestWinners;
using Clutch.Database.Entities.DirectMessages;
using Clutch.Database.Entities.DirectThreads;
using Clutch.Features.Contests.Shared;
using Clutch.Features.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Clutch.Infrastructure.Jobs;

// TODO: Implement email
public class FinalizeWeeklyContestJob(
    ApplicationDbContext dbContext) : IRecurringJob
{
    public async Task Execute(CancellationToken cancellationToken = default)
    {
        var contest = await CreateContestWithWinners();
        await NotifyWinnersByDirectThreads(contest.Winners);
        await NotifyWinnersByEmail(contest.Winners);
    }

    private async Task<Contest> CreateContestWithWinners()
    {
        var submissions = await dbContext.Clips
            .AsSplitQuery()
            .Include(clip => clip.Author)
            .GetInCurrentContest()
            .ToListAsync();

        var currentContestPeriod = ContestService.GetCurrentContestPeriod();

        var contest = new Contest
        {
            Start = currentContestPeriod.StartUtc,
            End = currentContestPeriod.EndUtc,
            SubmissionCount = submissions.Count,
            TotalLikes = submissions.Sum(submission => submission.LikeCount),
            TotalViews = submissions.Sum(submission => submission.ViewCount),
            Sponsors = [],
            Winners = submissions
               .OrderByDescending(submission => submission.LikeCount)
               .Take(5)
               .Select((top5, index) => new ContestWinner
               {
                   ClipId = top5.Id,
                   Clip = top5,
                   Placement = index + 1,
                   PrizeAmount = ContestService.GetPrizeAmountForPlacement(index + 1)
               }).ToList()
        };

        foreach (var winner in contest.Winners)
        {
            winner.Clip.Author.Wallet.Balance += winner.PrizeAmount;
        }


        dbContext.Contests.Add(contest);
        await dbContext.SaveChangesAsync();

        return contest;
    }
    private async Task NotifyWinnersByDirectThreads(List<ContestWinner> winners)
    {
        var clutch = await dbContext.Users
            .FirstAsync(x => x.DisplayName == "Clutch");

        foreach (var winner in winners)
        {
            var orderedParticipants = (clutch.Id, winner.Clip.AuthorId).Sort();

            var thread = await dbContext.DirectThreads
                .Include(t => t.Messages)
                .FirstOrDefaultAsync(t =>
                    t.ParticipantAId == orderedParticipants.Item1 &&
                    t.ParticipantBId == orderedParticipants.Item2);

            if (thread == null)
            {
                thread = new DirectThread
                {
                    ParticipantAId = orderedParticipants.Item1,
                    ParticipantBId = orderedParticipants.Item2,
                    Messages = []
                };

                dbContext.DirectThreads.Add(thread);
            }

            thread.Messages.Add(new DirectMessage
            {
                AuthorId = clutch.Id,
                Timestamp = DateTimeOffset.UtcNow,
                Text = ContestService.GetWinnerMessage(winner.Placement, winner.PrizeAmount),
                EventId = 0
            });

            await dbContext.SaveChangesAsync();
        }
    }


    private async Task NotifyWinnersByEmail(List<ContestWinner> winners)
    {

    }
}


