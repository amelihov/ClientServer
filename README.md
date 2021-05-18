# ClientServer
Develop a client-server chat consisting of
server and two network clients.
Server Requirements:
- .net core platform
- All infrastructure dependencies In-Memory
- No user authentication and authorization required

- The server must be able to receive commands in the terminal, arriving in the running
condition:
‣ exit command stops the server (closes the application)
‣ the ls command displays a list of open connections to the current server instance

- Optional: pack the image into docker

Console client requirements:
- .net core platform
- Communication with the server over a native TCP / IP socket without using HTTP and WebSocket.
- Real time data update

- When the console application starts, the user is prompted for his username. After
entering the username into the terminal, the last 30 messages are displayed (updated
when new ones arrive) and a cursor waiting to send a new message

Requirements for the web client:
- Any web framework (React / Angular / Vue)
- Real time data update

- Communication with the server via WebSocket
- When opening the page, the user is prompted for his username and then
a page / form with an active chat and a field for entering a new message opens

- Curved / oblique / standard html controls allowed.
