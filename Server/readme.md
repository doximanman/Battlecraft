# Server
The server uses websockets to communicate with clients.

How to run:<br/>
<pre>
python3 server.py <i>ip</i> <i>port</i> [<i>mongodb ip</i>] [<i>mongodb port</i>]
</pre>

## API:

### Ping -
* Ping - ping to check if the server is a valid battlecraft server
  * request: "ping"
  * response: "pong"

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
    * token: <i>authentication token</i>
  * response:
    * on success:
      * success: true
      * username: <i>username</i>
    * on failure:
      * success: false
      * message: <i>error message</i>


### World -
* Retrieve World Data
  * request:
    * type: "world"
    * subtype: "get"
    * token: <i>authentication token</i>
  * response:
    * if user exists and has world data:
      * success: true
      * data: <i>world data</i>
    * if user exists but has no world data:
      * success: true
      * data: "" (empty string)
    * if user doesn't exist:
      * success: false
      * message: <i>error message</i>


* Save World Data
  * request:
    * type: "world"
    * subtype: "save"
    * data: <i>world data</i>
    * token: <i>authentication token</i>
  * response:
    * on success:
      * success: true
    * on failure:
      * success: false
      * message: <i>error message</i>