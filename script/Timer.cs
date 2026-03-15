
namespace Utils
{
    public class DeltaTimer(double setTime)
    {
        private double resetTime = setTime;
        private double currentTime = setTime;

        public bool Delta(double removeTime)
        {
            currentTime -= removeTime;
            if (currentTime <= 0)
            {
                currentTime = resetTime;
                return true;
            }
            return false;
        }

        public void SetResetTime(double newTime)
        {
            resetTime = newTime;
        }
    }
}
