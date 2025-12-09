This program is built using ASP.NET. It's a conference API that manages talks as well as speakers of the same talks.

How to use?

==== Option 1 ====
1) install docker
2) Open docker
3) open the project folder in VS-CODE for this project and run the following commands
   a) docker build -t conference .
   b) docker run -p 8080:8080 conference
   c) visit this url http://localhost:8080
   
=== OTHER END POINTS YOU CAN VISIT ===
1) POST METHOD
   a) api/auth/register
   b) api/auth/login
   c) api/auth/refresh
   d) api/auth/revoke  
2) GET METHOD
   a) api/protected/talks
   b) api/protected/speakers
   c) api/protected/talks/{id}
   d) api/protected/speakers/{id}
3) PUT METHOD
   c) api/protected/talks/{id}
   d) api/protected/speakers/{id}
4) DELETE METHOD
   c) api/protected/talks/{id}
   d) api/protected/speakers/{id
