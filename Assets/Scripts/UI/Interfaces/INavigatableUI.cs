namespace UI
{
    public interface INavigatableUI : IGlobalUI
    {
        public void Down();
        public void Up();
        public void Right();
        public void Left();
    }
}