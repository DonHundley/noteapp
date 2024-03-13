# noteapp


An application built with the idea of storing notes from audio commands for Full-Stack AI project.

This application will utilize Docker, an Angular front end, websockets for the backend, and MySQL for the database. 

 ## Devlog UPDATE ONE

1. Basic back end features exist now, a user can become a journalist, subscribe to journal subjects, and add notes to them.
2. Basic testing and exception handling in place.
3. Docker works and doesn't. There is some issue with accessing the database when the app is ran in a container? idk. 
4. The database works though, so for now I will continue using MySQL but I will not run the actual application in docker.
5. Up next is fleshing out the backend, I want to cover any possible exceptions with events and have deeper testing.

## Devlog UPDATE TWO

1. JWT added
2. More robust error handling and better event coverage
3. small refactors to database schema as well as changes to repository methods to accomodate them.
4. Added test for logging in
5. Thoughts: I want to do more, at least it is better. Moving on to frontend now; It needs to be done so I can start FA&FO with voice-to-text AI. I imagine that will give me trouble.

## Devlog UPDATE THREE

1. Basic frontend functions work for the backend functions. Login, register, sub, and note-taking work.
2. I originally liked the idea of a timestamp for the notes, but this might be changed, idk.
3. Right now everything is on one page, this was to make sure everything was working right, time to separate.
4. Goals: split components, make a UI that feels closer to what I imagine the app should look like. 