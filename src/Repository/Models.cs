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

        public string Name { get; set; }
        public bool Computer { get; set; }
    }

    public class Board
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public ICollection<Movement> Movements { get; set; }
        public int NumberOfColumn { get; set; }
        public int NumberOfRows { get; set; }
    }

    public class Game
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Player Winner { get; set; }
        public bool Draw { get; set; }
        private bool Finished;
        public Board ConfiguredBoard { get; set; }

        public Game(Board board)
        {
            Finished = false;
            ConfiguredBoard = board;
        }

        public bool IsFinished()
        {
            return Finished;
        }
    }

    public class Movement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public int Position { get; set; }
        public Board Board { get; set; }
        public Player Player { get; set; }
    }
}