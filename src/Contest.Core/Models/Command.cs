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

    public class CommandSmove : Command, IMoveCommand
    {
        public CoordinateDifference d { get; }

        public CommandSmove(CoordinateDifference lld)
        {
            d = lld;
        }

        public override string ToString()
        {
            return $"SMove <{d.X}, {d.Y}, {d.Z}>";
        }

        public Voxel Destination(Voxel start, Matrix matrix)
        {
            return matrix.Translate(start, d);
        }
    }

    public class CommandLmove : Command, IMoveCommand
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
            return $"LMove <{d1.X}, {d1.Y}, {d1.Z}> <{d2.X}, {d2.Y}, {d2.Z}>";
        }

        public Voxel Destination(Voxel start, Matrix matrix)
        {
            return matrix.Translate(matrix.Translate(start, d1), d2);
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
            return $"Fill <{nd.X}, {nd.Y}, {nd.Z}>";
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
            return $"Void <{nd.X}, {nd.Y}, {nd.Z}>";
        }
    }

    public interface IMoveCommand
    {
        Voxel Destination(Voxel start, Matrix matrix);
    }
}