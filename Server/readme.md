# Server
The server uses websockets to communicate with clients.

How to run:<br/>
<pre>
python3 server.py <i>ip</i> <i>port</i> [<i>mongodb ip</i>] [<i>mongodb port</i>]
</pre>

## API:

* Message ping - ping to check if the server is a valid battlecraft server
* request:
  * type: "ping"
* response:
  * success: true


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
      * token: [token]
    * on failure:
      * success: false
      * message: [error message]


