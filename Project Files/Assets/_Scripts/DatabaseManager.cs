using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;

public class DatabaseManager : MonoBehaviour
{
    public PlaceManager placeManager;
    public PostManager postManager;
    public ProfileManager profileManager;
    
    public class Places
    {
        public string spotName;
        public string displayLocation;
        public string webLocation;
        public string fish;
        public string representImage;
        public double lat;
        public double lot;
        public int like;
    }
    public Places[] p;

    class Place
    {
        public string spotName;
        public string displayLocation;
        public string webLocation;
        public string fish;
        public string representImage;
        public double lat;
        public double lot;
        public int like;
        
        public Place (string spotName, string displayLocation, string webLocation, string fish, string representImage, double lat, double lot, int like)
        {
            this.spotName = spotName;
            this.displayLocation = displayLocation;
            this.webLocation = webLocation;
            this.fish = fish;
            this.like = like;
            this.lat = lat;
            this.lot = lot;
            this.representImage = representImage;
        }
    }

    public class Posts
    {
        public string userName;
        public string description;
        public string fileName;
        public string date;
        public int like;
    }
    public Posts[] posts;

    class Post
    {
        public string userName;
        public string description;
        public string fileName;
        public string date;
        public int like;
        
        public Post (string userName, string description, string fileName, string date, int like)
        {
            this.userName = userName;
            this.description = description;
            this.fileName = fileName;
            this.date = date;
            this.like = like;
        }
    }

    [System.Serializable]
    public class Comments
    {
        public string keyID;
        public string postID;
        public string userName;
        public string content;
        public string date;
    }
    public Comments[] comments;

    class Comment
    {
        public string keyID;
        public string postID;
        public string userName;
        public string content;
        public string date;
        
        public Comment (string keyID, string postID, string userName, string content, string date)
        {
            this.keyID = keyID;
            this.postID = postID;
            this.userName = userName;
            this.content = content;
            this.date = date;
        }
    }

    public DatabaseReference reference { get; set; }

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        ReadAll();
    }

    void ReadAll()
    {
        //Read Place
        int i = 0;
        reference = FirebaseDatabase.DefaultInstance.GetReference("Places");
        reference.GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                //Counting
                DataSnapshot snapshot = task.Result;
                foreach(DataSnapshot data in snapshot.Children)
                {
                    i++;
                }
                
                //Dynamic Assignment
                p = new Places[i];
                for (int a = 0; a < i; a++)
                {
                    p[a] = new Places();
                }
                
                //Init Array
                i = 0;
                foreach(DataSnapshot data in snapshot.Children)
                {                    
                    IDictionary place = (IDictionary)data.Value;
                    p[i].spotName = place["spotName"].ToString();
                    p[i].displayLocation = place["displayLocation"].ToString();
                    p[i].webLocation = place["webLocation"].ToString();
                    p[i].fish = place["fish"].ToString();
                    p[i].representImage = place["representImage"].ToString();
                    p[i].lat = float.Parse(place["lat"].ToString());
                    p[i].lot = float.Parse(place["lot"].ToString());
                    p[i].like = int.Parse(place["like"].ToString());
                    i++;
                }
                placeManager.CreateNewSpot(i);
                placeManager.Init_Popular();
            }
        });

        //Read Post
        int j = 0;
        reference = FirebaseDatabase.DefaultInstance.GetReference("Post");
        reference.GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                //Counting
                DataSnapshot snapshot = task.Result;
                foreach(DataSnapshot data in snapshot.Children)
                {
                    j++;
                }
                
                //Dynamic Assignment
                posts = new Posts[j];
                for (int a = 0; a < j; a++)
                {
                    posts[a] = new Posts();
                }
                
                //Init Array
                j = 0;
                foreach(DataSnapshot data in snapshot.Children)
                {                    
                    IDictionary post = (IDictionary)data.Value;
                    posts[j].userName = post["userName"].ToString();
                    posts[j].description = post["description"].ToString();
                    posts[j].fileName = post["fileName"].ToString();
                    posts[j].date = post["date"].ToString();
                    posts[j].like = int.Parse(post["like"].ToString());
                    j++;
                }
                postManager.InitPost(j);
                profileManager.doRefresh_myPost = true;
            }
        });
        
        //Read Comment
        int k = 0;
        reference = FirebaseDatabase.DefaultInstance.GetReference("Comment");
        reference.GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                //Counting
                DataSnapshot snapshot = task.Result;
                foreach(DataSnapshot data in snapshot.Children)
                {
                    k++;
                }
                
                //Dynamic Assignment
                comments = new Comments[k];
                for (int a = 0; a < k; a++)
                {
                    comments[a] = new Comments();
                }
                
                //Init Array
                k = 0;
                foreach(DataSnapshot data in snapshot.Children)
                {                    
                    IDictionary com = (IDictionary)data.Value;
                    comments[k].keyID = com["keyID"].ToString();
                    comments[k].postID = com["postID"].ToString();
                    comments[k].userName = com["userName"].ToString();
                    comments[k].content = com["content"].ToString();
                    comments[k].date = com["date"].ToString();
                    k++;
                }
            }
        });
    }

    public void ReadPostOnly()
    {
        int j = 0;
        reference = FirebaseDatabase.DefaultInstance.GetReference("Post");
        reference.GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                //Counting
                DataSnapshot snapshot = task.Result;
                foreach(DataSnapshot data in snapshot.Children)
                {
                    j++;
                }
                
                //Dynamic Assignment
                posts = new Posts[j];
                for (int a = 0; a < j; a++)
                {
                    posts[a] = new Posts();
                }
                
                //Init Array
                j = 0;
                foreach(DataSnapshot data in snapshot.Children)
                {                    
                    IDictionary post = (IDictionary)data.Value;
                    posts[j].userName = post["userName"].ToString();
                    posts[j].description = post["description"].ToString();
                    posts[j].fileName = post["fileName"].ToString();
                    posts[j].date = post["date"].ToString();
                    posts[j].like = int.Parse(post["like"].ToString());
                    j++;
                }
                postManager.InitPost(j);
                profileManager.doRefresh_myPost = true;
            }
        });

        int k = 0;
        reference = FirebaseDatabase.DefaultInstance.GetReference("Comment");
        reference.GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                //Counting
                DataSnapshot snapshot = task.Result;
                foreach(DataSnapshot data in snapshot.Children)
                {
                    k++;
                }
                
                //Dynamic Assignment
                comments = new Comments[k];
                for (int a = 0; a < k; a++)
                {
                    comments[a] = new Comments();
                }
                
                //Init Array
                k = 0;
                foreach(DataSnapshot data in snapshot.Children)
                {                    
                    IDictionary com = (IDictionary)data.Value;
                    comments[k].keyID = com["keyID"].ToString();
                    comments[k].postID = com["postID"].ToString();
                    comments[k].userName = com["userName"].ToString();
                    comments[k].content = com["content"].ToString();
                    comments[k].date = com["date"].ToString();
                    k++;
                }
                postManager.doRefresh_comment = true;
            }
        });
    }
    
    public void ReadCommentOnly(string path)
    {
        int k = 0;
        reference = FirebaseDatabase.DefaultInstance.GetReference("Comment");
        reference.GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                //Counting
                DataSnapshot snapshot = task.Result;
                foreach(DataSnapshot data in snapshot.Children)
                {
                    k++;
                }
                
                //Dynamic Assignment
                comments = new Comments[k];
                for (int a = 0; a < k; a++)
                {
                    comments[a] = new Comments();
                }
                
                //Init Array
                k = 0;
                foreach(DataSnapshot data in snapshot.Children)
                {                    
                    IDictionary com = (IDictionary)data.Value;
                    comments[k].keyID = com["keyID"].ToString();
                    comments[k].postID = com["postID"].ToString();
                    comments[k].userName = com["userName"].ToString();
                    comments[k].content = com["content"].ToString();
                    comments[k].date = com["date"].ToString();
                    k++;
                }
                postManager.doRefresh_comment = true;
            }
        });
    }
    
    [HideInInspector]
    public int like_count = 0;

    public void ReadLike(string category, string key)
    {
        FirebaseDatabase.DefaultInstance.GetReference(category).Child(key).Child("like").GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                like_count = int.Parse(snapshot.Value.ToString());
            }
            if (category == "Places")
                placeManager.doRefresh_like = true;
            else if (category == "Post")
                postManager.doRefresh_like = true;
        });
    }

    public void ReadLike_Details(string category, string key)
    {
        FirebaseDatabase.DefaultInstance.GetReference(category).Child(key).Child("like").GetValueAsync().ContinueWith(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                like_count = int.Parse(snapshot.Value.ToString());
            }
            postManager.doRefresh_like_details = true;
        });
    }

    void InitPlace()
    {
        AddPlace("가음저수지", "경상북도 의성군 가음면 양지리", "/의성군+가음면+가음저수지/@36.6469719,127.3357656/", "배스", "https://lh5.googleusercontent.com/p/AF1QipPA1SjaUE-JazQht0osQljxbC5bxSJdaHnReU5y=w426-h240-k-no", 36.2343138, 128.7383595, 4);
        AddPlace("반계저수지", "강원도 워주시 문막읍 반계리", "/원주시+문막읍+반계저수지/@37.411363,127.164938/", "배스", "https://lh5.googleusercontent.com/p/AF1QipNGwxp1Dt3Cej7N3RJLsmb8kooaLgQEd-0Y4P_F=w426-h240-k-no", 37.2930548, 127.764023, 0);
        AddPlace("동막저수지", "강원도 강릉시 구정면 어단리", "/강릉시+구정면+동막저수지/@37.5584611,127.3579494/", "배스", "https://lh5.googleusercontent.com/p/AF1QipN2XcRkfdW4deoNXVmAFnKcg9JUW_5rex4ROa0=w426-h240-k-no", 37.6919438, 128.9035511, 0);
        AddPlace("반송저수지", "강원도 춘천시 서면 월송리", "/춘천시+서면+반송저수지/@37.6390563,127.0289273/", "배스", "https://www.fishnet.co.kr/data/C040_1/200809/2038-1.jpg", 37.9278084, 127.6878841, 3);
        AddPlace("신매저수지", "강원도 춘천시 서면 서상리", "/춘천시+서면+신매저수지/@37.6439852,127.0650928/", "배스", "https://img.moolban.com/unsafe/1280x720/filters:no_upscale():watermark(https://img.moolban.com/unsafe/watermark_130.png,-10,-10,10)/company/images/7343/89d541cb882a1c065fec487baa7b616a.jpg", 37.9336102, 127.6434674, 0);
        AddPlace("소양호", "강원도 춘천시 신북읍", "/강원도+춘천시+신북읍+소양호/@37.6456564,126.7895061/", "배스+블루길", "https://lh5.googleusercontent.com/p/AF1QipOYZkEzmR43DNNL0kF2HRF5M-e6xeGHtQRSrIu1=w493-h240-k-no", 37.9451713, 127.8192711, 1);
        AddPlace("의암호", "강원도 춘천시 근화동", "/춘천시+근화동+의암호/@37.6253925,127.0643463/", "배스", "https://lh5.googleusercontent.com/p/AF1QipO3q2pcO4wqR4q32L5qajZkQMvJRWr4SrtQczyv=w408-h271-k-no", 37.890952, 127.7018561, 0);
        AddPlace("지내리저수지", "강원도 춘천시 신북읍", "/춘천시+신북읍+지내리저수지/@37.6573391,127.0349313/", "배스", "https://lh5.googleusercontent.com/p/AF1QipNIZxMEgv5TXqbQS5s752wB2WWQR4fZhMdylO9H=w426-h240-k-no", 37.9435294, 127.720936, 0);
        AddPlace("초당저수지", "강원도 삼척시 근덕면 하맹방리", "/삼척시+근덕면+초당저수지/@37.5560031,127.5268334/", "배스", "https://t1.daumcdn.net/blogfile/fs11/33_blog_2008_06_29_14_05_486717f71ea6b?x-content-disposition=inline&filename=초당007.jpg", 37.3871254, 129.1956713, 0);
        AddPlace("춘천호", "강원도 춘천시 사북면", "/춘천시+사북면+춘천호/@37.7052278,127.0723439/", "배스", "https://lh5.googleusercontent.com/p/AF1QipMuZQIyGLU92z6GYFOjSZgbV6YaeLyASX5TKEr3=w408-h306-k-no", 37.9753784, 127.6552533, 2);
    }

    public void AddPlace(string spotName, string displayLocation, string webLocation, string fish, string representImage, double lat, double lot, int like)
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        Place place = new Place(spotName, displayLocation, webLocation, fish, representImage, lat, lot, like);
        string json = JsonUtility.ToJson(place);
        reference.Child("Places").Child(place.spotName).SetRawJsonValueAsync(json);
    }

    public void LikeUpdate(string category, string key, string count)
    {
        reference = FirebaseDatabase.DefaultInstance.GetReference(category);
        string json = JsonUtility.ToJson(count);
        reference.Child(key).Child("like").SetRawJsonValueAsync(count);
    }

    public void AddPost(string userName, string description, string fileName, string date, int like)
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        Post post = new Post(userName, description, fileName, date, like);
        string json = JsonUtility.ToJson(post);
        reference.Child("Post").Child(date).SetRawJsonValueAsync(json);
    }

    public void AddComment(string postID, string userName, string content, string date)
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        string key = reference.Child("Comment").Push().Key;
        Comment comment = new Comment(key, postID, userName, content, date);
        string json = JsonUtility.ToJson(comment);
        reference.Child("Comment").Child(key).SetRawJsonValueAsync(json);
    }

    public void RemovePost(string date)
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("Post").Child(date).RemoveValueAsync();
    }

    public void RemoveComment(string key)
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("Comment").Child(key).RemoveValueAsync();
    }
}
