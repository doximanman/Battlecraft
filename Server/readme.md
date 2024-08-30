# Server
The server uses websockets to communicate with clients.

How to run:<br/>
<pre>
python3 server.py <i>ip</i> <i>port</i> [<i>mongodb ip</i>] [<i>mongodb port</i>]
</pre>

## API:

### Ping -
* Ping - ping to check if the server is a valid battlecraft server<br/>
  &nbsp;&nbsp;&nbsp;&nbsp;* request:<br/>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;* type: "ping"<br/>
  * response:
    * success: true

### User -

* Register user - register a user.
  * request:
    * type: "user"
    * subtype: "register"
    * username: <i>username</i>
    * password: <i>password</i>
  * response:
    * on success:
      * success: true
      * username: <i>username</i>
    * on failure:
      * success: false
      * message: <i>error message</i>


* Login user - login a user.
  * request:
    * type: "user"
    * subtype: "login"
    * username: <i>username</i>
    * password: <i>password</i>
  * response:
    * on success:
      * success: true
      * token: <i>token</i>
    * on failure:
      * success: false
      * message: <i>error message</i>


* Restore Login - try to restore a login using a token
  * request:
    * type: "user"
    * subtype: "login_with_token"
    * token: <i>token</i>
  * response:
    * on success:
      * success: true
      * username: <i>username</i>
    * on failure:
      * success: false
      * message: <i>error message</i>

### Player -
* Save Player Inventory
  * request:
    * type: "player"
    * subtype: "data"
    * token: <i>token of user</i>
  * response:
    * on success:
      * success: true
      * data: <i>player data</i>
    * on failure:
      * success: false
      * message: <i>error message</i>
