# Note-App, potential official name is "Commentate".


An application built with the idea of storing notes from audio commands for Full-Stack AI project.

In the current state the application works well, I would like to round it out by giving it more features and functions.

Concept: 
The inspiration for the application comes from a personal use, while I understand there may be some questions as to why
a user would want to have notes accessed by multiple users, it is something that my wife and I want for personal use.
Our current shared note taking solution is discord, and it just isn't good for that.

Some environmental variables will need to be set in order to use docker or the application, you can see these in the docker-compose. If TTS and REGION are left blank, the speech element will be disabled.

Then just use:

```Docker-compose up```

and access the app at:

```http://localhost:4200/login```

This application will have an event driven architecture. Other planned (or implemented) features:
1. I will utilize Docker-compose for the application and database. (DONE)
2. Angular front end with Quill and some primeng stuff I've seen Alex use; I liked the look of it for a style scheme and alerts. (DONE)
3. server side is C# and we are using websockets (DONE)
4. The database will be MySQL (DONE)
5. JWT for auth with features for controlling this on server and client side. (DONE)
6. Voice to text AI. (DONE)

Things I will handle with enough time:
1. I want to have better front-end exception coverage.
2. Fix that docker issue.
3. Implement MQTT, I haven't given this a ton of thought, but it would be nice to use the IoT skills.
4. Deployment. This will be done post deadline if necessary.
5. More modern feeling to the usage experience, I have an idea for automatic note saving/updating which will remove the need for buttons.
6. Voice based commands would be cool, I have an idea of how to do it too.

Things that won't happen:
1. Actually achieve satisfaction with the styling. SEND HELP.

Currently working with:
1. Bug hunting 2: electric dockeroo

 ## Dev-log UPDATE ONE

1. Basic back end features exist now, a user can become a journalist, subscribe to journal subjects, and add notes to them.
2. Basic testing and exception handling in place.
3. Docker works and doesn't. There is some issue with accessing the database when the app is run in a container? IDK. 
4. The database works though, so for now I will continue using MySQL, but I will not run the actual application in docker.
5. Up next is fleshing out the backend, I want to cover any possible exceptions with events and have deeper testing.

## Dev-log UPDATE TWO

1. JWT added
2. More robust error handling and better event coverage
3. small refactors to database schema as well as changes to repository methods to accommodate them.
4. Added test for logging in
5. Thoughts: I want to do more, at least it is better. Moving on to frontend now; It needs to be done, so that I can start FA&FO with voice-to-text AI. I imagine that will give me trouble.

## Dev-log UPDATE THREE

1. Basic frontend functions work for the backend functions. Login, register, sub, and note-taking work.
2. I originally liked the idea of a timestamp for the notes, but this might be changed, IDK.
3. Right now everything is on one page, this was to make sure everything was working right, time to separate.
4. Goals: split components, make a UI that feels closer to what I imagine the app should look like. 

## Dev-log UPDATE FOUR

1. Front-end seperated into components based on function.
2. added some styling, got annoyed with styling. All my homies hate styling.
3. subjects page looks pretty rough, I'll mess with it some more but see the above note.
4. Did I mention I'm annoyed with styling?
5. Happy with the progress though, there are some functions I want to add, but I really need to move onto tackling AI for the deadline.
6. Up next: finish styling and move on to implementing AI on both ends of the application.

## Dev-log UPDATE FIVE
1. The front-end is looking nice for the moment. Swapped to using primeng for a general design.
2. split notes/subjects into their own component, I feel it is a better design and solved a big headache.
3. Upgraded security, we have a guard now and our jwt has a life cycle of 4 hours. 
4. The application will redirect the user to the login if the server emits the correct error too. 
5. Looking into using Quill for the note-taking instead of my custom setup, if it gives a major problem I'm just moving to the AI.
6. next: Quill? then AI. 

## Dev-log UPDATE SIX
1. Quill is implemented, I really like it. 
2. Unfortunately I now wish I fully implemented CRUD functions for notes, because my front-end feels so nice that I want them now. R.I.P. me.
3. Next? still not AI, this has to have full functions; It feels like a disservice to the effort. Mostly hours messing with scss.
4. I used to be annoyed with styling. I still am, but I used to be too.

## Dev-log UPDATE SEVEN
1. Done with implementing current features on the front-end.
2. styling is acceptable.
3. New note functions are implemented, and can be updated in what feels like a moderately natural way.
4. Next: No more putting it off, AI.

## Dev-log UPDATE EIGHT

1. Text to speech works on the back end.
2. Developed a test for TTS.
3. TTS disables if env vars are not set.
4. Messed with Docker some more, I'm still not sure what the issue is. Still just using docker-compose up db and dotnet run.

## Dev-log UPDATE NINE

1. I've been calling it text to speech, even my variables are text to speech, my brain is broken and the speech sdk is to blame.
2. It is finished, and it works. A Journalist can speak and after a short delay their note will be added. 
3. The documentation for the SDK is a nightmare and feels outdated.
4. Next? Bug hunting before presentation. Docker is the main target, I would really like to fix that issue. 

## Dev-log UPDATE TEN

1. Docker issue fixed!
2. Application is presentation ready, I'm quite happy  with it and most everything went to schedule.
3. Next? Post project development will probably slow, but I will spend more time with it.