using System;
using Akka;
using Akka.Actor;
using Akka.Event;

namespace Actors
{
    public class AnimalSounds : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public AnimalSounds()
        {
            Receive<Play>(_ =>
            {
                _log.Debug("I'm not animal");
            });

            Receive<Swap>(_ =>
            {
                Become(Dog);
            });

            Receive<Push>(_ =>
            {
                BecomeStacked(Dog);
            });

            Receive<Pop>(_ =>
            {
                UnbecomeStacked();
            });
        }

        private void Dog()
        {
            Receive<Play>(_ =>
            {
                _log.Debug("멍멍");
            });

            Receive<Swap>(_ =>
            {
                Become(Cat);
            });

            Receive<Push>(_ =>
            {
                BecomeStacked(Cat);
            });

            Receive<Pop>(_ =>
            {
                UnbecomeStacked();
            });
        }

        private void Cat()
        {
            Receive<Play>(_ =>
            {
                _log.Debug("야옹");
            });

            Receive<Pop>(_ =>
            {
                UnbecomeStacked();
            });
        }

        public static Props Props()
        {
            return Akka.Actor.Props.Create<AnimalSounds>();
        }
    }
}