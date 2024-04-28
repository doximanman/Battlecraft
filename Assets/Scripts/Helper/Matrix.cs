using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Matrix of objects.
/// Rows can be of different lengths.
/// </summary>
/// <typeparam name="T">Type of the objects</typeparam>
[System.Serializable]
public class Matrix<T> : IEnumerable<List<T>>
{
    [SerializeField] private List<ListWrapper<T>> _matrix;

    /// <summary>
    /// Initialize a matrix with default values
    /// </summary>
    /// <param name="rows">number of rows</param>
    /// <param name="cols">number of columns</param>
    public Matrix(int rows, int cols)
    {
        _matrix = new();
        for(int i = 0; i < rows; i++)
        {
            _matrix.Add(new());
            for(int j = 0; j < cols; j++)
            {
                _matrix[i].list.Add(default);
            }
        }
    }

    /// <summary>
    /// Initialize a matrix from an enumerable of enumerables.
    /// Each enumerable is a row.
    /// </summary>
    /// <param name="values">values to initialize</param>
    public Matrix(IEnumerable<IEnumerable<T>> values)
    {
        Assert.IsTrue(values != null);

        _matrix = new();
        
        for(int i = 0; i < values.Count(); i++)
        {
            if (values.ElementAt(i) == null)
            {
                // list wrapper with empty list
                _matrix.Add(new());
            }
            else
            {
                int len = values.ElementAt(i).Count();
                // list wrapper with empty list
                _matrix.Add(new());
                for (int j = 0; j < len; j++)
                {
                    _matrix[i].list.Add(values.ElementAt(i).ElementAt(j));
                }
            }
        }
    }
    
    
    /// <summary>
    /// Return the number of rows
    /// </summary>
    /// <returns>number of rows</returns>
    public int Count()
    {
        return _matrix.Count();
    }

    public bool ContainedBy(IEnumerable<IEnumerable<T>> other)
    {
        if (other == null) return _matrix == null;
        if (_matrix.Count > other.Count()) return false;

        for (int i = 0; i < _matrix.Count; i++)
        {
            if (other.ElementAt(i) == null && _matrix[i] != null)
                return false;
            if (_matrix[i].list.Count > other.ElementAt(i).Count()) return false;
        }

        return true;
    }

    /// <summary>
    /// Size of this matrix is bigger than the other matrix.
    /// Meaning: each row of this matrix is at least as long as the 
    /// row at that position in 'other'.
    /// Doesn't compare values.
    /// </summary>
    /// <param name="other"></param>
    /// <returns>whether the length of each row in this matrix is bigger the length of the same row in 'other'</returns>
    public bool Contains(IEnumerable<IEnumerable<T>> other)
    {
        if (other == null) return _matrix == null;
        if (_matrix.Count < other.Count()) return false;

        for (int i = 0; i < _matrix.Count; i++)
        {
            if (other.ElementAt(i) == null && _matrix[i] != null)
                return false;
            if (_matrix[i].list.Count < other.ElementAt(i).Count()) return false;
        }

        return true;
    }

    /// <summary>
    /// Size of this matrix is equal to the size of the other matrix.
    /// size = length of rows and length of each row.
    /// </summary>
    /// <param name="other">the other matrix</param>
    /// <returns>whether the number of rows and the length of each row is the same between the matrices.</returns>
    public bool SameStructure(IEnumerable<IEnumerable<T>> other)
    {
        if (other == null && _matrix==null) return _matrix==null;
        if(_matrix.Count!=other.Count()) return false;

        for(int i=0;i< _matrix.Count; i++)
        {
            if (other.ElementAt(i) == null && _matrix[i] != null)
                return false;
            if (_matrix[i].list.Count != other.ElementAt(i).Count()) return false;
        }

        return true;
    }

    /// <summary>
    /// Turns this matrix into a 1 dimensional list.
    /// </summary>
    /// <returns>List of the rows of the matrix, appended in order</returns>
    public List<T> Flatten()
    {
        List<T> result = new();

        if (_matrix == null) return result;
        for (int i = 0; i < _matrix.Count; i++)
        {
            result.AddRange(_matrix[i].list);
        }
        return result;
    }

    /// <summary>
    /// Element at position
    /// </summary>
    /// <param name="i">position</param>
    /// <returns>element at position 'i'</returns>
    public List<T> this[int i]
    {
        get { return _matrix[i].list; }
        set { _matrix[i].list = value; }
    }

    public override bool Equals(object obj)
    {
        if (obj is Matrix<T> other)
        {
            if (other._matrix.Count != _matrix.Count) return false;
            for (int i = 0; i < _matrix.Count; i++)
            {
                if (_matrix[i].list.Count != other._matrix[i].list.Count) return false;
                for (int j = 0; j < _matrix[i].list.Count; j++)
                {
                    if (!_matrix[i].list[j].Equals(other._matrix[i].list[j])) return false;
                }
            }

            return true;
        }
        else if (obj is IEnumerable<IEnumerable<T>> altOther)
        {
            if (altOther.Count() != _matrix.Count) return false;
            for (int i = 0; i < _matrix.Count; i++)
            {
                if (_matrix[i].list.Count != altOther.ElementAt(i).Count()) return false;
                for (int j = 0; j < _matrix[i].list.Count; j++)
                {
                    if (!_matrix[i].list[j].Equals(altOther.ElementAt(i).ElementAt(j))) return false;
                }
            }

            return true;
        }
        return _matrix == null;
    }

    public override int GetHashCode()
    {
        return _matrix == null ? 0 : _matrix.Count+1;
    }

    IEnumerator<List<T>> IEnumerable<List<T>>.GetEnumerator()
    {
        for(int i=0;i< _matrix.Count; i++)
        {
            yield return _matrix[i].list;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        for (int i = 0; i < _matrix.Count; i++)
        {
            yield return _matrix[i].list;
        }
    }
}
