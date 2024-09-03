from expiring_dict import ExpiringDict
from motor.motor_asyncio import AsyncIOMotorClient

key: str = ""
db: AsyncIOMotorClient
# used to cache token decodes.
# useful for frequent decoding.
# values expire (are removed from the dict) after 10 seconds
token_decode_cache: ExpiringDict = ExpiringDict(ttl=10)
sockets: dict[str, AsyncIOMotorClient] = {}