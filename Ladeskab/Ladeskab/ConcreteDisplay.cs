using System;

namespace Ladeskab
{
    public class ConcreteDisplay : IDisplay
    {
        public void Display(string content)
        {
            Console.WriteLine(content);
        }
    }
}