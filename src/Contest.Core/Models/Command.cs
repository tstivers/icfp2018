namespace Contest.Core.Models
{
    public class Command
    {
    }

    public class CommandHalt : Command
    {
        public override string ToString()
        {
            return "Halt";
        }
    }

    public class CommandWait : Command
    {
        public override string ToString()
        {
            return "Wait";
        }
    }

    public class CommandFlip : Command
    {
        public override string ToString()
        {
            return "Flip";
        }
    }

    public class CommandSmove : Command
    {
        private CoordinateDifference _lld;

        public CoordinateDifference d => _lld;

        public CommandSmove(CoordinateDifference lld)
        {
            _lld = lld;
        }

        public override string ToString()
        {
            return $"SMove <{_lld.x}, {_lld.y}, {_lld.z}>";
        }
    }

    public class CommandLmove : Command
    {
        private CoordinateDifference _sld1;
        private CoordinateDifference _sld2;

        public CoordinateDifference d1 => _sld1;
        public CoordinateDifference d2 => _sld2;

        public CommandLmove(CoordinateDifference sld1, CoordinateDifference sld2)
        {
            _sld1 = sld1;
            _sld2 = sld2;
        }

        public override string ToString()
        {
            return $"LMove <{_sld1.x}, {_sld1.y}, {_sld1.z}> <{_sld2.x}, {_sld2.y}, {_sld2.z}>";
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

        public CoordinateDifference nd => _nd;

        public CommandFill(CoordinateDifference nd)
        {
            _nd = nd;
        }

        public override string ToString()
        {
            return $"Fill <{_nd.x}, {_nd.y}, {_nd.z}>";
        }
    }
}