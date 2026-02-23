## About This Project

Clutch is a backend architecture experiment where I explore
event-driven design, high-throughput ingestion, and scalable
materialization patterns using SQL and background processing.

It is not intended as a finished product, but as a technical
playground to test architectural tradeoffs.

---
 
 Clutch

A TikTok-style backend built in .NET 10 focused on event-driven architecture, scalability patterns, and production-style infrastructure design.

This project is not just a CRUD demo. It explores how a short-form video platform could be built using:

* Event logging
* Compaction
* Eventual consistency
* Cursor-based pagination
* Snowflake IDs
* Background materialization
* Azure-based media pipeline (optional)

—


When the application starts for the first time, the database is automatically seeded with:
Demo Account
Email:    clutchaccount@gmail.com
Password: ClutchPassword123***
Demo Clip
A single clip is seeded into the database:
Clip ID: 1
Game: Counter-Strike 2
Description: "Insane Donk highlight."

How To Observe The Event-Driven Architecture
After starting the API:
Register with a new account and login


Like the demo clip (ClipId = 1) using:


POST /clip/like
Example payload:
{
 "clipId": 1,
 "likeType": 0,
 "deviceId": "demo-device",
 "platform": "web",
 "browserLanguage": "en",
 "devicePlatform": "windows"
}
Observe what happens:


A raw event is appended to the UserEvents table.


The materializer background job processes the event.


The Likes table and counters are updated asynchronously.


Consumer offsets advance.


You can inspect:
UserEvents (append-only event log)


EventConsumerOffsets


Likes


Clips.LikeCount


This allows you to see the full ingestion > compaction > materialization pipeline in action.
---

# Architecture Overview

Clutch is designed around an append-only event log.

Instead of directly mutating entities like `Likes`, `Follows`, `Saves`, etc., the system:

1. Appends raw events to a write-optimized event table.
2. Compacts events per entity key.
3. Materializes final state asynchronously.
4. Maintains consumer offsets per consumer group.

This mimics a simplified Kafka-style pipeline using SQL + background jobs.

---

# Why Raw SQL For Events?

Event ingestion is write-heavy and append-only.

EF Core was benchmarked using NBomber and compared against raw SQL for write-only workloads.

Results showed raw SQL to be significantly more efficient for high-throughput event ingestion scenarios.

Because this event table:

* Is append-only
* Does not require tracking
* Does not need change detection
* Does not need navigation graphs

Raw SQL is a better fit than EF Core for this specific workload.

All other domain modeling continues to use EF Core.

---

# Event Compaction

Since user interactions (likes, follows, saves) can toggle repeatedly, the event log is compacted before materialization.

Example:

```
Like
Unlike
Like
```

Compaction ensures only the final state is applied.

This prevents:

* Redundant DB operations
* Inflated counters
* Materialization inefficiencies

Each consumer group maintains its own offset.

---

# Eventual Consistency

The system intentionally embraces eventual consistency.

Why?

User interactions are stored as raw events first. Materialized state (likes table, follow table, counters, inboxes) is updated asynchronously.

This design:

* Optimizes write throughput
* Prevents lock contention
* Enables horizontal scaling
* Decouples ingestion from materialization

For a social-media-style app, this tradeoff is acceptable.

---

# Snowflake IDs

Entities are assigned Snowflake IDs.

Why?

Events are written before materialization occurs, and entities may need IDs before being persisted in their final form.

Snowflake IDs:

* Are globally unique
* Are time-ordered
* Allow horizontal scaling
* Avoid database round-trips for ID generation
* Work well in distributed systems

This is particularly useful in high-scale social applications.

---

# Media Pipeline (Optional Infrastructure)

The project includes an Azure-based media pipeline:

* Blob upload
* EventGrid trigger
* Azure Function transcoding
* Callback to API (`/transcoding-complete`)
* Staging → final Clip publishing

This infrastructure is included to demonstrate production-style media ingestion.

However:

* Azure resources are not required to run the backend.
* Blob storage and transcoding are not wired for local usage.
* Clip uploads are currently disabled without Azure setup.

The architecture is present to demonstrate how it would function in production.

---

# Tech Stack

* .NET 10
* EF Core
* Raw SQL for event ingestion
* Hangfire (background jobs)
* Immediate (minimal API handler abstraction)
* Snowflake ID generation (IdGen)
* Polly (resilience)
* Azure Blob Storage (optional)
* Azure Functions (optional)

---

# Running Locally

Requirements:

* .NET 10 SDK
* SQL Server (LocalDB works fine)

Steps:

1. Update your connection string in `appsettings.Development.json` (or just run)
2. Apply migrations
3. Run the API

---

# Usage Instructions

1. Register a new user:

   * `POST /register`
   * Provide email + password

2. Log in:

   * `POST /login`
   * Receive authentication token

3. After logging in you can:

   * Follow other users
   * Send direct messages
   * Like content
   * Save content
   * View inbox activity
   * Trigger event-driven interactions

Note:
Clip uploads are currently disabled unless Azure Blob Storage + Azure Function pipeline is configured.

---

# Project Intent

This project was built to explore:

* High-throughput event ingestion
* SQL-based event sourcing patterns
* Compaction strategies
* Background materialization
* Infrastructure-aware backend design

It is intentionally built with production-style concerns in mind rather than as a minimal CRUD demo.

---

# Future Improvements

* Replace SQL event log with Kafka or similar stream platform
* Add Redis caching layer for hot counters
* Add Azurite-based local blob emulator
* Add containerized development environment
* Introduce CQRS read models

---

# Closing Notes

Clutch is an architectural experiment in building a scalable social backend in .NET.

It prioritizes:

* Write throughput
* Decoupling
* Scalability patterns
* Clean domain modeling
* Infrastructure realism

---


