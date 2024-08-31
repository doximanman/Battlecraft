import string
from hashlib import sha256
import service.token as token_service
from globals import db
import re

users_db = db['Users']


async def verify_user(username: str, password: str):
    # user is stored by their username and the hash of their password
    passwordHash = sha256(password.encode()).hexdigest()
    # find the user in the db
    result = await users_db.find_one({
        'username': username,
        'passwordHash': passwordHash
    })

    # result = None means the user wasn't found.
    # otherwise, it is simply not none.
    return result is not None


async def gen_token(username: str, password: str):
    # the token stores the username and the hash of their password
    passwordHash = sha256(password.encode()).hexdigest()
    userdata = {
        'username': username,
        'passwordHash': passwordHash,
    }
    return token_service.gen_token(userdata)


async def verify_with_token(token: str):
    preUser = token_service.verify_token(token)

    # verify token returns boolean 'success'.
    # if 'success' is false, it also returns an error message.
    # if 'success' is true, it also returns the data encoded in the token.
    if preUser['success']:
        user = preUser['data']
    else:
        return preUser

    exists = await user_exists(user['username'])
    if exists:
        return {
            'success': True,
            'username': user['username']
        }
    else:
        return {
            'success': False,
            'message': 'User not found'
        }


async def user_exists(username: str):
    result = await users_db.find_one({'username': username})
    return result is not None


async def create_user(username: str, password: str):
    # credentials validity check

    # at most 20 characters
    if len(username) > 20 or len(password) > 20:
        return {
            "success": False,
            "message": "Credentials must be at most 20 characters long"
        }

    # at least 8 characters
    if len(username) < 8 or len(password) < 8:
        return {
            'success': False,
            'message': 'Credentials must be at least 8 characters long'
        }

    # username can only have letters and numbers and the symbols _ and -
    check = '[a-zA-Z0-9_-]+'
    username_check = re.fullmatch(check, username)
    if username_check is None:
        return {
            'success': False,
            'message': 'Username can only have letters, numbers and the symbols - and _'
        }

    # password can only have letters and numbers and symbols
    check = '[a-zA-Z0-9' + string.punctuation + ']+'
    password_check = re.fullmatch(check, password)
    if password_check is None:
        return {
            'success': False,
            'message': 'Password can only have letters, numbers and symbols'
        }

    # password must include at least one letter, number and symbol
    checkLetter, checkNumber, checkSymbol = '[a-zA-Z]', '[0-9]', '[' + string.punctuation + ']'
    letter, number, symbol = re.search(checkLetter, password), re.search(checkNumber, password), re.search(checkSymbol,
                                                                                                           password)
    if letter is None or number is None or symbol is None:
        return {
            'success': False,
            "message": "Password must contain at least one letter, number and symbol"
        }

    # check user doesn't exist
    if await user_exists(username):
        return {
            'success': False,
            "message": "User already exists"
        }

    # all valid! create the user
    await users_db.insert_one({
        'username': username,
        'passwordHash': sha256(password.encode()).hexdigest(),
    })

    return {
        'success': True,
        'username': username
    }
