
namespace Amegakure.Starkane.InputSystem
{
    public interface ICommand 
    {
        public string Name { get; }
        public void Do();
    }
}
