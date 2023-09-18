

public class ExpeUser{
    public int userId {get; set;}
    public int group {get; set;}


    public ExpeUser(int userId_, int group_){
        userId = userId_;
        group = group_;
    }


    public static ExpeUser FromUserChannel(UserChannel userChannel){
        return new ExpeUser(userChannel.user.id, userChannel.user.group);
    }

}