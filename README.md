# RESPONSE DTOs

This section contains all the object types that can be returned or given in a request body.

```typescript
DtoUser {
  id: string,
  email: string,
  name: string,
  public: boolean,
  creationDate: string,
  likedSongs: string[],
  dislikedSongs: string[]
}

DtoUserPublic {
  id: string,
  name: string,
  creationDate: string,
  public: boolean,
}

DtoUserUpdate {
  name: string,
  public: boolean,
}

DtoSong {
  id: string,
  name: string,
  lyrics: string,
  description: string,
  accessFor: number,
  creationDate: string,
  lastUpdateDate: string
  user: DtoUser
}

DtoSongAdd {
  name: string,
  lyrics: string,
  accessFor: number,
  description: string
}

DtoSongUpdate {
  id: string,
  name: string,
  lyrics: string,
  description: string,
  accessFor: number
}

DtoSongPublic {
  id: string,
  name: string,
  lyrics: string,
  description: string,
  creationDate: string,
  lastUpdateDate: string,
  likes: number,
  dislikes: number,
  comments: number,
  user: DtoUserPublic
}

DtoCommentAdd {
  content: string,
  songId: string
}

DtoCommentPublic {
  id: string,
  content: string,
  creationDate: string,
  edited: boolean,
  user: DtoUserPublic,
  songId: string
}

DtoFollowStatus {
  followStatus: 0 | 1 | 2 //(0 = Not following, 1 = Requested to follow, 2 = Following)
}

DtoFollowPublic {
  followStatus: 0 | 1 | 2
  followId: string
  id: string
  user: DtoUserPublic
  date: string
}
```

# Endpoints
This section contains all the API endpoints with descriptions, bodies, parameters and responses.

**POST Users/Add**
- DESCRIPTION: Adds new user to the database based on google data (email, name)
- BODY: No body, auth data is taken from auth header
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 400: User already exist.
  - 200: DtoUser

**GET Users/GetUserData**
- DESCRIPTION: Returns the data for the connected user.
- QUERY PARAMETERS: No parameters, it knows what user to return based on the idToken.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Not found.
  - 200: DtoUser

**POST Users/Update**
- DESCRIPTION: Updates connected user data (currently name and public visibility)
- BODY: DtoUserUpdate
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 400: User doesn't exist.
  - 200: DtoUser

**POST Users/ToggleRating/{ratingName}/{songId}**
- DESCRIPTION:
- BODY: No body, it identifies what rating to toggle based on URL parameters.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 400: Endpoint must contain either Like or Dislike.
  - 404: User doesn't exist.
  - 200: DtoUser

**GET Users/GetUserPublic/{userId}**
- DESCRIPTION:
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Not found.
  - 200: DtoUserPublic

**GET Songs/GetByUser**
- DESCRIPTION: Returns the list of projects/songs for the connected user.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 200: DtoSong[]

**GET Songs/GetByIdPublic/{id}**
- DESCRIPTION: Returns the song/project for a given id.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Connected user doesn't exist.
  - 400: Song doesn't exist.
  - 401: You don't have access to this song because it's private or because you are not a follower.
  - 200: DtoSongPublic

**POST Songs/Add**
- DESCRIPTION: Creates a new song/project for the connected user.
- BODY: DtoSongAdd
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 400: User doesn't exist.
  - 200: DtoSong

**DELETE Songs/Delete/{id}**
- DESCRIPTION: Deletes a song/project for the connected user based on a given id.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 400: Song doesn't exist.
  - 401: You can't delete stuff from other users.
  - 200: DtoSong

**POST Songs/Update**
- DESCRIPTION: Updates a song/project for the connected user.
- BODY: DtoSongUpdate
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 400: User doesn't exist.
  - 400: Song doesn't exist.
  - 400: You can't edit data from other users.
  - 200: DtoSong

**GET Songs/GetPublic**
- DESCRIPTION: Returns the list of public songs.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 400: Database issue.
  - 200: DtoSongPublic[]

**GET Songs/GetByUserId/{userId}**
- DESCRIPTION: Returns the list of public songs and songs the connected user has access to (if they are a follower of the given user)
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Connected user doesn't exist.
  - 200: DtoSongPublic[]

**POST Comments/Add**
- DESCRIPTION: Adds a new comment to a post.
- BODY: DtoCommentAdd
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: User doesn't exist.
  - 404: Song doesn't exist.
  - 400: Comment must be at least 2 characters long without whitespace.
  - 200: DtoCommentPublic

**GET Comments/GetBySongId/{songId}**
- DESCRIPTION: Returns the list of comments for a given song/post.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: User doesn't exist.
  - 200: DtoCommentPublic[]

**DELETE Comments/Delete/{commentId}**
- DESCRIPTION: Deletes a comment by commentId.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: User doesn't exist.
  - 404: Comment doesn't exist.
  - 401: You can't delete comments from other users.
  - 200: DtoCommentPublic

**GET Follows/GetFollowStatus/{userId}**
- DESCRIPTION: Returns the follow status of the connected user who accesses the profile of a given user (0 = Not following, 1 = Requested to follow, 2 = Following)
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Connected User doesn't exist.
  - 200: DtoFollowStatus

**GET Follows/GetFollowers**
- DESCRIPTION: returns the list of followers for the connected user.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Connected User doesn't exist.
  - 200: DtoFollowPublic[]

**POST Follows/AddFollowRequest/{userId}**
- DESCRIPTION: Sends a follow request to a given user.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Connected User doesn't exist.
  - 404: Account that User wants to follow doesn't exist.
  - 200: DtoFollowStatus

**DELETE Follows/Unfollow/{userId}**
- DESCRIPTION: Connected user unfollows a given user.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Connected User doesn't exist.
  - 404: You are not following that user.
  - 200: Ok.

**POST Follows/AcceptFollowRequest/{userId}**
- DESCRIPTION: Connected user accepts a follow request for a given user.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Connected User doesn't exist.
  - 404: Follow request doesn't exist.
  - 200: Ok.

**DELETE Follows/DeleteFollower/{userId}**
- DESCRIPTION: Connected user removes a follower based on userId.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Connected User doesn't exist.
  - 404: Follower doesn't exist.
  - 200: Ok.

**GET dexonline.ro/{type}/{word}/json**
- DESCRIPTION: Returns the definitions/synonyms/antonyms for a given word.