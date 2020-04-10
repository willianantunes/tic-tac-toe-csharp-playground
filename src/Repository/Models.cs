using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using src.Helper;

namespace src.Repository
{
    public class Group
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        public Guid GroupId { get; set; }
        public Group Group { get; set; }
    }

    public class TodoItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }

    public class Player
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
    }

    public class Board
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int NumberOfColumn { get; set; }
        public int NumberOfRows { get; set; }

    }

    public class Game
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public ICollection<Movement> Movements { get; set; }

        private bool Finished;
        private Board ConfiguredBoard;

        public Game(Board board)
        {
            Finished = false;
            ConfiguredBoard = board;
        }

        public bool IsFinished()
        {
            return Finished;
        }

        public bool PositionIsNotAvailable(in int movementPosition)
        {
            var copiedMovementPosition = movementPosition;
            var foundMovement = Movements.First(m => m.Position.Equals(copiedMovementPosition));

            if (foundMovement.IsNotNull())
                return true;

            return AvailablePositions().Contains(copiedMovementPosition);
        }

        public IList<int> AvailablePositions()
        {
            var availablePosition = new List<int>();
            var positionCount = 1;
            
            for (int column = 1; column <= ConfiguredBoard.NumberOfColumn; column++)
            {
                for (int row = 1; row <= ConfiguredBoard.NumberOfRows; row++)
                {
                    var hasNoPosition = Movements.None(m => m.Position == positionCount);
                    
                    if (hasNoPosition)
                        availablePosition.Add(positionCount);
                    
                    positionCount++;
                }
            }

            return availablePosition;
        }
    }

    public class Movement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public int Position { get; set; }
        public Game Game { get; set; }
    }
}