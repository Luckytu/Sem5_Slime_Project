using Slime;

namespace InstagramGeneration
{
    public class InstagramSlime
    {
        public bool useFakeData { get; set; }
        public string userName { get; set; }
        public int follower { get; set; }
        public int following { get; set; }
        public int posts { get; set; }
        public float lifeTime { get; set; }
        public Species species { get; set; }

        public InstagramSlime(string userName, int follower, int following, int posts, Species species)
        {
            useFakeData = false;
            this.userName = userName;
            this.follower = follower;
            this.following = following;
            this.posts = posts;
            lifeTime = 0f;
            this.species = species;
        }
    }
}