# Server
The server uses websockets to communicate with clients.

How to run:<br/>
```angular2html
python3 server.py [ip] [port]
```

## API:
* Register user - register a user. socket is closed on response.
  * request:
    * type: "user"
    * subtype: "register"
    * username: [username]
    * password: [password]
  * response:
    * on success:
      * success: true
      * username: [username]
    * on failure:
      * success: false
      * message: [error message]


* Login user - login a user.  socket is closed on response.
  * request:
    * type: "user"
    * subtype: "login"
    * username: [username]
    * password: [password]
  * response:
    * on success:
      * success: true
      * JWT: [token]
    * on failure:
      * success: false
      * message: [error message]


