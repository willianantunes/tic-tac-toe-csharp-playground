using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
    }
    
    public class Game
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public bool isFinished()
        {
            throw new NotImplementedException();
        }

        public bool PositionIsNotAvailable(in int movementPosition)
        {
            throw new NotImplementedException();
        }

        public IList<int> AvailablePositions()
        {
            throw new NotImplementedException();
        }
    }
}