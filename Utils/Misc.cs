using Microsoft.EntityFrameworkCore;
using VersuriAPI.Data;
using VersuriAPI.Models;

namespace VersuriAPI.Utils
{
    public static class Misc
    {

        public static bool ContainsDuplicates<T>(List<T> list)
        {
            var seen = new HashSet<T>();
            foreach (var item in list)
            {
                if (!seen.Add(item))
                    return true;
            }
            return false;
        }

        public static User? getUserByEmail(AppDbContext dbContext, string email)
        {
            var foundUser = dbContext.Users.Where(u => u.Email == email).FirstOrDefault();
            return foundUser;
        }

        public static void ToggleFromList<T>(List<T> list, T element)
        {
            if (list.Contains(element))
            {
                list.Remove(element);
            }
            else
            {
                list.Add(element);
            }
        }

        public static DtoUserPublic UserToPublic(User user)
        {
            var userPublic = new DtoUserPublic
            {
                Id = user.Id,
                Name = user.Name,
                CreationDate = user.CreationDate,
                Public = user.Public
            };

            return userPublic;
        }

        public static DtoSongPublic SongToPublic(AppDbContext dbContext, Song song)
        {

            var songPublic = new DtoSongPublic
            {
                Id = song.Id,
                Name = song.Name,
                Lyrics = song.Lyrics,
                Description = song.Description,
                CreationDate = song.CreationDate,
                AccessFor = song.AccessFor,
                LastUpdateDate = song.LastUpdateDate,
                Likes = dbContext.Users.Count(u => u.LikedSongs.Contains(song.Id)),
                Dislikes = dbContext.Users.Count(u => u.DislikedSongs.Contains(song.Id)),
                Comments = dbContext.Comments.Count(c => c.Song.Id == song.Id),
                User = UserToPublic(song.User)
            };

            return songPublic;
        }

        public static DtoCommentPublic CommentToPublic(Comment comment)
        {
            var commentPublic = new DtoCommentPublic
            {
                Id = comment.Id,
                Content = comment.Content,
                CreationDate = comment.CreationDate,
                Edited = comment.Edited,
                User = UserToPublic(comment.User),
                SongId = comment.Song.Id,
            };

            return commentPublic;
        }

        public static DtoFollowPublic FollowToPublic(Follow follow)
        {

            var followPublic = new DtoFollowPublic
            {
                FollowsId = follow.FollowsId,
                User = UserToPublic(follow.User),
                Id = follow.Id,
                FollowStatus = follow.FollowStatus,
            };

            return followPublic;
        }
    }
}
