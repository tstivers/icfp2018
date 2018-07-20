namespace Contest.Core.Models
{
    public class Command
    {
    }

    public class CommandHalt : Command
    {
    }

    public class CommandWait : Command
    {
    }

    public class CommandFlip : Command
    {
    }

    public class CommandSmove : Command
    {
        private CoordinateDifference _lld;

        public CommandSmove(CoordinateDifference lld)
        {
            _lld = lld;
        }
    }

    public class CommandLmove : Command
    {
        private CoordinateDifference _sld1;
        private CoordinateDifference _sld2;

        public CommandLmove(CoordinateDifference sld1, CoordinateDifference sld2)
        {
            _sld1 = sld1;
            _sld2 = sld2;
        }
    }

    public class CommandFission : Command
    {
        private CoordinateDifference _nd;
        private int _m;

        public CommandFission(CoordinateDifference nd, int m)
        {
            _nd = nd;
            _m = m;
        }
    }

    public class CommandFill : Command
    {
        private CoordinateDifference _nd;

        public CommandFill(CoordinateDifference nd)
        {
            _nd = nd;
        }
    }
}