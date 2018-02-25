using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoTS_Service.Util
{
    public class AnimatedBar
    {
        List<string> animation;
        int counter;
        public AnimatedBar()
        {
            this.animation = new List<string> { "/", "-", @"\", "|" };
            this.counter = 0;
        }

        /// <summary>
        /// prints the character found in the animation according to the current index
        /// </summary>
        public void Step(int value)
        {
            Console.Write(this.animation[this.counter] + " " + value + "\r");
            this.counter++;
            if (this.counter == this.animation.Count)
                this.counter = 0;
        }
    }
}
