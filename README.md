# Skillustrator

Welcome to Skillustrator, which allows organizations to the track skills of their people, get deeper insight into their organization's overall abilities, what kind of tasks and projects can be taken on, and where gaps are. 

It allows skill tagging which opens up many scenarios that it can support. For example:
* Tag skills into a general category, such as Angular and Javascript into the tag *Front-end*. This way, you can find all the skills your company has for front-end, or *Needed Front-end* for all the skills your company needs for front-end projects.
* Tag skills with a project name, to indicate which skills are needed for what types of projects, or even clients. 
* Tag skills with jr, mid, sr to indicate what skills are required at what seniorty level. 

It is fully Dockerized with an Angular 2 front-end, ASP.NET Core API, and PostgreSQL database, each in their own containers that can be spun up with just 1 docker-compose command (more below).

Please contribute! See our [kanban board](https://github.com/excellalabs/skillustrator/projects/1) for stories.

## How it works 

Users log into their profile and for each skill they have, they rank their level of expertise, the amount of time they used the skill, and when the last time was they used the skill. 

Skill(s) can be searched for, bringing back who has what skill or combination of skills, what skill level they have, and how current they are.

Skills can be added and tagged via the skills & tags management screens.

## Base features

* Allow people within a company (or potentially even publicly) to list their skillsets in a quick and easy way. It provides a public skills profile very simple for users to complete.
* Surface visibility of skillsets when you need a certain skill, or combination of skills, for something
* Get an overall idea of skillsets across a company. Visibility into cross-pollination efforts, etc.
* Find Users with a list of Skills (i.e. build a team)
* Build your next Team
* Determine how current you are with your skills 

## Features under consideration

* Visualizing Skills
    * Individual skillsets as a set of bubbles or boxes along the following lines:
    * Expertise/proficiency level = color
    * Length of time the skill's been used = size of the box/bubble
    * Last time the skill was used = opacity (faded if it's been a while).
* Pull users from anywhere (LDAP, OAuth, etc.)
* API to interface (think LinkedIn widget, StackCareers integration, etc.)
* Suggest skills based on things other people are adding
* Find users with similar skills / like-minded users
* Compare your organization to others
* Add LinkedIn and StackCareer Profiles to your Skillustrator Profile (cannibalizes their limited skill lists)
* "What type of organization are you?" -- see what range of skills you have, where you're weak, how you compare to other companies
* Skillsume -- nice printed version of your skill resume (version for color, version for black & white) -- skills by category or tag?
* Facebook integration -- post your skills to your profile or page, find friends with similar skillsets
* Widget for about.me
* Personal resume -- clouds for different colors, skill length of experience is length of block, faded colors on the outside of the cloud.

# Running the app

The entire app will spin up - UI (Angular 2), API (ASP.NET Core) and database (PostgreSQL) each in their own container. 

**NOTE for Windows users**: 
    a) you must have the new bash installed (comes with Windows 10 Anniversary Update), and set up a C share in Docker settings. 
    b) **If you get an error that it can't run go.sh,* use git bash's (or install it in any bash) dos2unix go.sh to convert the Windows line endings to Unix for the container.

1. If you haven't built yet, run: 

    `docker-compose build`

1. To start the containers & the app, run: 

    `docker-compose up -d`

The UI should be running 
(http://localhost:4200/person/999, test user seeded there).

## Operations

1. If you need to log into the container (i.e. run migrations or troubleshoot), use this command. Use `docker ps` to list running containers and see the name:

    `docker exec -it <container_name> bash`

1. Run this to *stop the app & containers*. Simply run the up command above to restart them:

    `docker-compose stop`

    *NOTES:* 
    If you want to stop and destory the containers (and will also delete the database server & data), run `docker-compose down`. You can see what Docker images you have with `docker images`, and what *running* containers you have with `docker ps` (put -a on to see them all). 
    
    You might want to delete containers and images. Please refer to `docker rm` and `docker rmi` for how.

1. To see the console output within the container, run `docker logs <container_name>`

1. To run the tests for the API, use `docker exec -it <container_name> dotnet tests`

## Add a database migration when model changes 

1. Log into the app container (see instructions above). There is also a script available API/scpirts/appContainerLogin.sh that runs the below.

1. Run `dotnet ef migrations add <name>`

1. Run `dotnet ef database update`

## Manage the database 

### via pgAdmin 

Download and point it to localhost. Make sure the port matches what's being exposed from the Postgres container in the docker-compose file. 

### via psql CLI

1. Log into the postgres container (see instructions above)

1. Run `psql -h localhost -p 5432 -d skillustrator -U postgres --password`. Enter `password`.

1. To list tables in the database type, `\dt`. You can do other database operations here. 

1. To exit type, `\q`

**NOTE:** you can also you a GUI tool to manage the database like **pgAdmin**

## NOTE: To run this Angular 2/Angular-CLI UI outside of Docker:

First, you will need Node 4 or higher, as well as NPM 3 or higher (download here https://nodejs.org/en/download/).

Second, install Angular-CLI globally:
```bash
npm install -g angular-cli
```

Third, create an entry in your computer's `HOSTS` file:
```api   127.0.0.1
```

Run the following commands to install dependencies and create a server:

```bash
cd UI
npm install
npm start
```

NOTE: "ng serve" is the standard start-up command for Angular CLI, however you must run "npm start" to ensure back-end API calls work properly (see https://github.com/angular/angular-cli#proxy-to-backend).


## Specs

- Database
    - Local: PostgreSQL, Production: Azure SQL
    - DB Management: [PgAdmin](https://www.pgadmin.org/). When Postgres container is up and running you can connect with PgAdmin to localhost:5432. Alternatively, you can bash to the postgres container and use [psql](https://www.postgresql.org/docs/9.2/static/app-psql.html).
- API: 
    - ASP.NET Core Web API in a Docker container
    - Data layer: EF Core 1.0, repository pattern
- UI: Angular 2 in a Docker container

## Additional NOTES

### To run containers individually 

1. If you haven't built it, run:

    `docker build -t msexcella:aspnetcore-dev-env . `

1. Then run: 

    ```
    docker run -d --name postgres -e POSTGRES_PASSWORD=password postgres

    docker run -d -p 5000:5000 --link my-postgres:postgres msexcella:aspnetcore-dev-env 
    ```
