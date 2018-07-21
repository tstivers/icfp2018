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
        public CoordinateDifference d { get; }

        public CommandSmove(CoordinateDifference lld)
        {
            d = lld;
        }

        public override string ToString()
        {
            return $"SMove <{d.x}, {d.y}, {d.z}>";
        }
    }

    public class CommandLmove : Command
    {
        public CoordinateDifference d1 { get; }

        public CoordinateDifference d2 { get; }

        public CommandLmove(CoordinateDifference sld1, CoordinateDifference sld2)
        {
            d1 = sld1;
            d2 = sld2;
        }

        public override string ToString()
        {
            return $"LMove <{d1.x}, {d1.y}, {d1.z}> <{d2.x}, {d2.y}, {d2.z}>";
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
        public CoordinateDifference nd { get; }

        public CommandFill(CoordinateDifference nd)
        {
            this.nd = nd;
        }

        public override string ToString()
        {
            return $"Fill <{nd.x}, {nd.y}, {nd.z}>";
        }
    }

    public class CommandVoid : Command
    {
        public CoordinateDifference nd { get; }

        public CommandVoid(CoordinateDifference nd)
        {
            this.nd = nd;
        }

        public override string ToString()
        {
            return $"Void <{nd.x}, {nd.y}, {nd.z}>";
        }
    }
}