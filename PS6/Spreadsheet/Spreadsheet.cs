﻿//Written by Josh Christensen u0978248 
using System;
using System.Collections.Generic;
using System.Linq;
using SpreadsheetUtilities;
using System.Xml;
using System.Text.RegularExpressions;

namespace SS
{

    /// <summary>
    /// An Spreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a cell name if and only if it consists of one or more letters,
    /// followed by one or more digits AND it satisfies the predicate IsValid.
    /// For example, "A15", "a15", "XY032", and "BC7" are cell names so long as they
    /// satisfy IsValid.  On the other hand, "Z", "X_", and "hello" are not cell names,
    /// regardless of IsValid.
    /// 
    /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    /// must be normalized with the Normalize method before it is used by or saved in 
    /// this spreadsheet.  For example, if Normalize is s => s.ToUpper(), then
    /// the Formula "x3+a5" should be converted to "X3+A5" before use.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
        /// A DependencyGraph object recording all the dependency pairs for this object.
        /// </summary>
        private DependencyGraph dependencies = new DependencyGraph();
        /// <summary>
        /// A dictionary containing all of the non-empty cells in this object.
        /// </summary>
        private Dictionary<String, Cell> nonEmptyCells = new Dictionary<string, Cell>();
        /// vinc: CircularDependency pool containing all involved cells
        private HashSet<string> CircularDependency = new HashSet<string>();
        ///// vinc: event to refresh panel
        //public event refreshPanel myRefreshPanel;

        /// <summary>
        /// Denotes if this spreadsheet has changed since it's construction, or last save.
        /// </summary>
        private bool hasChanged;

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get
            {
                return hasChanged;
            }

            protected set
            {
                hasChanged = value;
            }
        }

        /// <summary>
        /// Creates a spreadsheet with no extra validity constraints, normalized every cell 
        /// name to itself, and has the version "default".
        /// </summary>
        public Spreadsheet() : this(s => true, s => s, "default")
        {
            //Don't do anything special
        }

        /// <summary>
        /// Constructs a spreadsheet by recording its variable validity test,
        /// its normalization method, and its version information.  The variable validity
        /// test is used throughout to determine whether a string that consists of one or
        /// more letters followed by one or more digits is a valid cell name.  The variable
        /// equality test should be used thoughout to determine whether two variables are
        /// equal.
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            hasChanged = false;
        }

        private string RemoveWhitespace(string str)
        {
            return Regex.Replace(str, @"\s+", "");
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            name = Normalize(name);
            IsNameInvalidOrNull(name);
            if (GetCellContents(name).GetType().Equals(typeof(Formula)))
            {
                //return ((Formula)GetCellContents(name)).Evaluate(Lookup);
                object result = ((Formula)GetCellContents(name)).Evaluate(Lookup);
                //if (result is InvalidFormat)
                //{
                //    result = "InvalidFormat:" + ((InvalidFormat)result).message;
                //}
                return result;
            }
            else
            {
                return GetCellContents(name);
            }
        }

        /// <summary>
        /// If the varname passed as a string evaluates as a double, returns that double. Else, throws
        /// and ArgumentException.
        /// </summary>
        /// <param name="varname"></param>
        /// <returns></returns>
        private double Lookup(string varname)
        {
            object result = GetCellValue(varname);
            if (result.GetType().Equals(typeof(double)))
            {
                return (double)result;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            //Return all the names of our cells in our dictionary
            return nonEmptyCells.Keys;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            name = Normalize(name);//Normalize name
            IsNameInvalidOrNull(name); //If we get passed this, our name is valid
            //If this isn't a NonEmptyCell, it's value is the empty string.
            if (!GetNamesOfAllNonemptyCells().Contains(name))
            {
                return "";
            }
            else
            {
                return nonEmptyCells[name].Contents;
            }

        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content, bool isInvalidFormatAllowed)
        {
            if (ReferenceEquals(content, null))
            {
                throw new ArgumentNullException();
            }

            name = Normalize(name);
            IsNameInvalidOrNull(name);//Check if the name is invalid, or null. Throws exception if true.

            ISet<string> output = null; //We should either replace this, or throw exception. 

            double parsedContent;
            if (content.Equals("")) //Special case for empty strings (they break the formula checker)
            {
                output = SetCellContents(name, content);
            }
            else if (content[0].Equals('=')) //If we're a formula
            {
                Formula formula = new Formula(content.Substring(1), Normalize, IsValidVarName);
                if (!formula.ValidFormat)
                {
                    if (isInvalidFormatAllowed)
                    {
                        output = SetCellContents(name, formula);
                    }else
                    {
                        throw new InvalidFormatException(formula.formaterror.message);
                    }
                }else
                {
                    output = SetCellContents(name, formula);
                }
            }
            else if (Double.TryParse(content, out parsedContent)) //If we're a double
            {
                output = SetCellContents(name, parsedContent);
            }
            else //Else, we're just a string.
            {
                output = SetCellContents(name, content);
            }

            hasChanged = true; //We're past all exceptions at this point.
            return output;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            IsNameInvalidOrNull(name); //If we get past this, our name is valid

            MakeEmpty(name);//Make our cell empty.

            nonEmptyCells.Add(name, new Cell(name, number)); //Add our new cell

            HashSet<string> returnValue = new HashSet<String>(GetCellsToRecalculate(name));//Get our list of cells to recalculate

            return returnValue; //Return all the cells to recalculate.
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            IsNameInvalidOrNull(name); //If we get past this, our name is valid

            if (text == null)
            {
                throw new ArgumentNullException();
            }

            MakeEmpty(name); //Make the cell empty.

            //If the cell's value is going to be empty, don't re-add to the dictionary
            if (text.Equals(""))
            {
                return new HashSet<String>(GetCellsToRecalculate(name)); //Return all the cells to recalculate.
            }
            else
            {
                nonEmptyCells.Add(name, new Cell(name, text)); //Add our new cell
                return new HashSet<String>(GetCellsToRecalculate(name)); //Return all the cells to recalculate.
            }

        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            IsNameInvalidOrNull(name); //If we get past this, our name is valid

            if (formula == null)
            {
                throw new ArgumentNullException();
            }

            // vinc: handle InvalidFormat
            if (!formula.ValidFormat)
            {
                //nonEmptyCells.Add(name, new Cell(name, formula)); //Add our new cell
                nonEmptyCells[name] = new Cell(name, formula);
                HashSet<string> result = new HashSet<string>();
                result.Add(name);
                return result;
            }

            //Preserve the old value.
            object oldValue = GetCellContents(name);

            MakeEmpty(name); //Make the cell empty.

            //Add all our dependencies.
            foreach (string varName in formula.GetVariables())
            {
                dependencies.AddDependency(varName, name);
            }

            nonEmptyCells.Add(name, new Cell(name, formula)); //Add our new cell

            IEnumerable<string> returnValue = null; //This should never exit the method as null. We throw an exeption, or replace it.

            //Check to see if the new cell will create a circular dependency
            try
            {
                returnValue = GetCellsToRecalculate(name);
                foreach(string cellName in returnValue)
                {
                    CircularDependency.Remove(cellName);
                    ((Formula)nonEmptyCells[cellName].Contents).CircularDependency = false;
                }
            }
            catch (CircularException e)
            {
                foreach(string cellName in e.dependencies)
                {
                    CircularDependency.Add(cellName);
                    ((Formula)nonEmptyCells[cellName].Contents).CircularDependency = false;
                }
                formula.CircularDependency = true;
                return new HashSet<String>(e.dependencies);
            }

            return new HashSet<String>(returnValue); //All is well, return.
        }



        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException();
            }

            IsNameInvalidOrNull(name); //If we get past this, our name is valid

            return dependencies.GetDependents(name); //Return our dependents.
        }

        /// <summary>
        /// If the passed name is invalid (does not follow the established rules of variable names)
        /// or null, throws an InvalidNameException with an apropriate name.
        /// </summary>
        /// <param name="name">The name to check the validity of.</param>
        private void IsNameInvalidOrNull(string varname)
        {
            if (varname == null)
            {
                throw new InvalidNameException();
            }

            if (!IsValidVarName(varname))
            {
                throw new InvalidNameException(); //The variable must pass our valid variable name test.
            }

            //If we haven't broken the rules, its valid.
        }

        /// <summary>
        /// Our internal validator. Checks both our standard definition of a variable, as well as validity under the IsValid functor.
        /// We check the former, then the latter.
        /// </summary>
        /// <param name="varname">The variable name to check.</param>
        /// <returns></returns>
        private bool IsValidVarName(string varname)
        {
            if (varname.Length == 0)
            {
                return false;//Variables can't be empty
            }
            bool hasNumbers = false;//A variable must have trailing numbers
            int pos;
            //Check any number of starting letters. 
            for (pos = 0; pos < varname.Length; pos++)
            {
                if (!Char.IsLetter(varname[pos]))
                {
                    if (pos == 0)
                    {
                        return false;//Our first element must be a letter
                    }
                    else if (char.IsDigit(varname[pos]))
                    {
                        break; //Now, check digits
                    }
                    else
                    {
                        return false; //If we encounter a non letter + non digit, variable is invalid.
                    }
                }
            }
            //Check any number of numbers, after we finish letters.
            for (pos = pos; pos < varname.Length; pos++)
            {
                hasNumbers = true;
                if (!Char.IsDigit(varname[pos]))
                {
                    return false;//If we encounter a non-digit at this point, variable is invalid.
                }
            }

            if (!hasNumbers)
            {
                return false;//The variable didn't have any numbers in it.
            }

            return IsValid(varname); //If we haven't broken the rules, then we just have to check the delegate.
        }

        /// <summary>
        /// Makes the specified cell empty. If it exists and depends on cells, removes those dependencies. Has no error checking.
        /// </summary>
        /// <param name="name"></param>
        private void MakeEmpty(string name)
        {
            //If the cell exists
            if (GetNamesOfAllNonemptyCells().Contains(name))
            {
                //If the cell is a formula.
                if (nonEmptyCells[name].Contents.GetType() == typeof(Formula))
                {
                    Formula toBeRemoved = (Formula)nonEmptyCells[name].Contents;
                    //Remove the dependencies for every varName in this cell.
                    foreach (string varName in toBeRemoved.GetVariables())
                    {
                        dependencies.RemoveDependency(varName, name);
                    }
                }
                nonEmptyCells.Remove(name); //Remove it from our dictionary
            }
        }

        /// <summary>
        /// Represents a cell on a spreadsheet. 
        /// 
        /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
        /// contents is an empty string, we say that the cell is empty.
        /// 
        /// Considering all returned types are immutable, at this time, cells are immutable.
        /// </summary>
        private class Cell
        {
            /// <summary>
            /// The contents of this cell. Must be a string, a double, or a Formula.
            /// </summary>
            object contents;


            /// <summary>
            /// The name of this cell. Used only internally.
            /// </summary>
            string name;

            /// <summary>
            /// Creates a new Cell object, using the parameter as Contents. 
            /// The parameter must fit the definition of Contents, else an ArgumentException is thrown.
            /// </summary>
            /// 
            /// <param name="name">The name of this cell.</param>
            /// <param name="contents">The value to set to be contents.</param>
            public Cell(string name, object contents)
            {
                this.Contents = contents;
                this.name = name;
            }

            /// <summary>
            /// The contents of this cell. Must be a string, a double, or a Formula. If this property is set to anything else, 
            /// an ArgumentException is thrown.
            /// </summary>
            public object Contents
            {
                set
                {
                    if (value.GetType().Equals(typeof(string)))
                    {
                        this.contents = value;
                    }
                    else if (value.GetType().Equals(typeof(double)))
                    {
                        this.contents = value;
                    }
                    else if (value.GetType().Equals(typeof(Formula)))
                    {
                        this.contents = value;
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
                get
                {
                    return contents;
                }
            }
        }
    }



    /// <summary>
    /// 
    /// </summary>
    public class InvalidFormatException : Exception
    {
        public InvalidFormatException(string message) : base(message)
        {
        }
    }

    public delegate void refreshPanel();
}
