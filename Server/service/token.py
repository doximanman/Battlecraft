import jwt
from globals import key, token_decode_cache


def verify_token(token: str):
    if token in token_decode_cache:
        data = token_decode_cache[token]
        return{
            'success': True,
            'data': data
        }
    try:
        data = jwt.decode(token, key, algorithms=['HS256'])
    except jwt.exceptions.ExpiredSignatureError:
        return {
            'success': False,
            'message': 'Token expired'
        }
    except jwt.exceptions.InvalidTokenError:
        return {
            'success': False,
            'message': 'Invalid token'
        }

    # add to cache
    token_decode_cache[token] = data
    return {
        'success': True,
        'data': data
    }

def gen_token(data):
    token = jwt.encode(data, key, algorithm='HS256')
    token_decode_cache[token] = data
    return token