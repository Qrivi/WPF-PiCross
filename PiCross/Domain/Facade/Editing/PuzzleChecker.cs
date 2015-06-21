using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PiCross.Actors;
using PiCross.Cells;
using PiCross.DataStructures;
using PiCross.Game;

namespace PiCross.Facade.Editing
{
    public class PuzzleChecker : IPuzzleChecker
    {
        private readonly PuzzleCheckerActor actor;

        public PuzzleChecker()
        {
            actor = new PuzzleCheckerActor();
        }

        public void FindAmbiguities( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints, Cell<IGrid<bool>> output )
        {
            actor.Check( columnConstraints: columnConstraints, rowConstraints: rowConstraints, output: output );
        }

        public void Kill()
        {
            actor.Kill();
        }

        private class PuzzleCheckerActor
        {
            private readonly IMessageQueue<IMessage> messageQueue;

            private bool alive;

            private SolverGrid solverGrid;

            private Cell<IGrid<bool>> output;

            public PuzzleCheckerActor()
            {
                this.messageQueue = CreateMessageQueue();
                this.alive = true;

                CreateAndStartWorkerThread();
            }

            private void CreateAndStartWorkerThread()
            {
                new Thread( this.ThreadProcedure ) { IsBackground = true, Name = "Puzzle Checker Actor Thread", Priority = ThreadPriority.Lowest }.Start();
            }

            private static IMessageQueue<IMessage> CreateMessageQueue()
            {
                var messageQueue = new MessageQueue<IMessage>();
                var synchronizedMessageQueue = new SynchronizedMessageQueue<IMessage>( messageQueue );
                return new BlockingMessageQueue<IMessage>( synchronizedMessageQueue );
            }

            private void ThreadProcedure()
            {
                while ( alive )
                {
                    var message = messageQueue.Inbox.Receive();

                    message.Execute();
                }
            }

            public void Check(ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints, Cell<IGrid<bool>> output)
            {
                columnConstraints = columnConstraints.Copy();
                rowConstraints = rowConstraints.Copy();

                Enqueue( () => PerformCheck(columnConstraints, rowConstraints, output ) );
            }

            private void PerformCheck( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints, Cell<IGrid<bool>> output )
            {
                if ( this.output != null )
                {
                    this.output.Value = null;
                }

                this.output = output;

                this.solverGrid = new SolverGrid( columnConstraints, rowConstraints );

                Enqueue( PerformSolveStep );
            }

            private void PerformSolveStep()
            {
                var before = solverGrid.CountUnknowns();
                solverGrid.SinglePassRefine();
                var after = solverGrid.CountUnknowns();

                if ( before == after )
                {
                    var result = solverGrid.Squares.Map( square => square == Square.UNKNOWN ).Copy();

                    output.Value = result;
                    output = null;
                    solverGrid = null;
                }
                else
                {
                    Enqueue( PerformSolveStep );
                }
            }

            public void Kill()
            {
                Enqueue( PerformKill );
            }

            private void PerformKill()
            {
                if ( this.output != null )
                {
                    this.output.Value = null;
                    this.output = null;
                }

                this.solverGrid = null;
                alive = false;
            }

            private void Enqueue( IMessage message )
            {
                messageQueue.Outbox.Send( message );
            }

            private void Enqueue( Action action )
            {
                Enqueue( new MethodCall( action ) );
            }
        }

        private interface IMessage
        {
            void Execute();
        }

        private class MethodCall : IMessage
        {
            private readonly Action action;

            public MethodCall( Action action )
            {
                this.action = action;
            }

            public void Execute()
            {
                action();
            }
        }
    }
}
