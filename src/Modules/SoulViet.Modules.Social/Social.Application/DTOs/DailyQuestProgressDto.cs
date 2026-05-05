namespace SoulViet.Modules.Social.Social.Application.DTOs;

public class DailyQuestProgressDto
{
    public QuestProgressItem LikeQuest { get; set; } = new();
    public QuestProgressItem CommentQuest { get; set; } = new();
    public QuestProgressItem ShareQuest { get; set; } = new();
    public int CreatorPostsCount { get; set; }
    public int MaxCreatorPosts { get; set; } = 3;
}

public class QuestProgressItem
{
    public int Current { get; set; }
    public int Target { get; set; } = 10;
    public bool IsRewarded { get; set; }
}
