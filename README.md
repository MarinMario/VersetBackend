# Setup

**Things to install**
- Visual Studio
- Git
- NPM
- ASP.net and web development (from Visual Studio installer)

**Backend Setup**
1. Clone repository
    - `git clone https://github.com/MarinMario/VersetBackend.git`
2. Initialize database
    - `dotnet ef migrations add InitialCreate`
    - `dotnet ef database update`
3. Run the app in http mode

**Frontend setup**
1. Clone repository
    - `git clone https://github.com/MarinMario/VersetFrontend.git`

2. Install dependencies
    - `cd VersetFrontend`
    - `npm install`

3. Add environment file `.env.development` in the root directory
    ```python
    VITE_API_URL="http://localhost:5074/api"
    VITE_CLIENT_ID="client id from google console"
    VITE_CLIENT_SECRET="client secret from google console"
    ```

4. Run the app
    - `npm run dev`

# RESPONSE DTOs
This section contains all the object types that can be returned or given in a request body.

```typescript
DtoUser {
  id: string,
  email: string,
  name: string,
  : boolean,
  creationDate: string,
  likedSongs: string[],
  dislikedSongs: string[]
}

DtoUser {
  id: string,
  name: string,
  creationDate: string,
  : boolean,
}

DtoUserUpdate {
  name: string,
  : boolean,
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

DtoSong {
  id: string,
  name: string,
  lyrics: string,
  description: string,
  creationDate: string,
  lastUpdateDate: string,
  likes: number,
  dislikes: number,
  comments: number,
  user: DtoUser
}

DtoCommentAdd {
  content: string,
  songId: string
}

DtoComment {
  id: string,
  content: string,
  creationDate: string,
  edited: boolean,
  user: DtoUser,
  songId: string
}

DtoFollowStatus {
  followStatus: 0 | 1 | 2 //(0 = Not following, 1 = Requested to follow, 2 = Following)
}

DtoFollow {
  followStatus: 0 | 1 | 2
  id: string
  user: DtoUser
  follows: DtoUser
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
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Not found.
  - 200: DtoUser

**POST Users/Update**
- DESCRIPTION: Updates connected user data (currently name and  visibility)
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

**GET Users/GetUser/{userId}**
- DESCRIPTION:
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Not found.
  - 200: DtoUser

**GET Songs/GetByUser**
- DESCRIPTION: Returns the list of projects/songs for the connected user.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 200: DtoSong[]

**GET Songs/GetById/{id}**
- DESCRIPTION: Returns the song/project for a given id.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Connected user doesn't exist.
  - 400: Song doesn't exist.
  - 401: You don't have access to this song because it's private or because you are not a follower.
  - 200: DtoSong

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

**GET Songs/Get**
- DESCRIPTION: Returns the list of  songs.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 400: Database issue.
  - 200: DtoSong[]

**GET Songs/GetByUserId/{userId}**
- DESCRIPTION: Returns the list of  songs and songs the connected user has access to (if they are a follower of the given user)
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Connected user doesn't exist.
  - 200: DtoSong[]

**POST Comments/Add**
- DESCRIPTION: Adds a new comment to a post.
- BODY: DtoCommentAdd
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: User doesn't exist.
  - 404: Song doesn't exist.
  - 400: Comment must be at least 2 characters long without whitespace.
  - 200: DtoComment

**GET Comments/GetBySongId/{songId}**
- DESCRIPTION: Returns the list of comments for a given song/post.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: User doesn't exist.
  - 200: DtoComment[]

**DELETE Comments/Delete/{commentId}**
- DESCRIPTION: Deletes a comment by commentId.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: User doesn't exist.
  - 404: Comment doesn't exist.
  - 401: You can't delete comments from other users.
  - 200: DtoComment

**GET Follows/GetFollowStatus/{userId}**
- DESCRIPTION: Returns the follow status of the connected user who accesses the profile of a given user (0 = Not following, 1 = Requested to follow, 2 = Following)
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Connected User doesn't exist.
  - 200: DtoFollowStatus

**GET Follows/GetFollowers**
- DESCRIPTION: Returns the list of followers for the connected user.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Connected User doesn't exist.
  - 200: DtoFollow[]

**GET Follows/GetFollowing**
- DESCRIPTION: Returns the list of users that the connected user follows.
- RESPONSES:
  - 401: Authorization Token is Invalid.
  - 404: Connected User doesn't exist.
  - 200: DtoFollow[]

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

# Database schema

**Users table**
```typescript
  Id: string                // uuid
  Email: string
  Name: string
  Public: boolean
  CreationDate: string      // datetime
  LikedSongs: string[]      // list of uuid,
  DislikedSongs: string[]   // list of uuid,
```

**Songs table**
```typescript
  Id: string                // uuid
  Name: string
  Lyrics: string 
  Description: string
  CreationDate: string      // datetime
  LastUpdateDate: string    // datetime
  AccessFor: number         // 0 | 1 | 2 
  UserId: string            // uuid 
```

**Comments table**
```typescript
  Id: string                // uuid 
  Content: string 
  CreationDate: string      // datetime
  Edited: boolean
  UserId: string            // uuid
  SongId: string            // uuid
```

**Follows table**
```typescript
  Id: string                // uuid
  UserId: string            // uuid
  FollowsId: string         // uuid
  Date: string              // datetime
  FollowStatus: number      // 0 | 1 | 2
```