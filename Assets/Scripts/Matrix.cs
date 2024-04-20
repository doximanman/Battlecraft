using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class Matrix<T>
{
    public List<ListWrapper<T>> _matrix;

    public Matrix(int rows, int cols)
    {
        _matrix = new(rows);
        for(int i = 0; i < rows; i++)
        {
            _matrix[i].list = new(cols);
            for(int j = 0; j < cols; j++)
            {
                _matrix[i].list[j] = default;
            }
        }
    }

    public Matrix(IEnumerable<IEnumerable<T>> values)
    {
        Assert.IsTrue(values != null);

        _matrix = new(values.Count());
        
        for(int i = 0; i < _matrix.Count; i++)
        {
            if (values.ElementAt(i) == null)
            {
                _matrix[i].list = new(0);
            }
            else
            {
                int len = values.ElementAt(i).Count();
                _matrix[i].list = new(len);
                for (int j = 0; j < len; i++)
                {
                    _matrix[i].list[j] = values.ElementAt(i).ElementAt(j);
                }
            }
        }
    }
    
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
            if (_matrix.Count > other.ElementAt(i).Count()) return false;
        }

        return true;
    }

    public bool Contains(IEnumerable<IEnumerable<T>> other)
    {
        if (other == null) return _matrix == null;
        if (_matrix.Count < other.Count()) return false;

        for (int i = 0; i < _matrix.Count; i++)
        {
            if (other.ElementAt(i) == null && _matrix[i] != null)
                return false;
            if (_matrix.Count < other.ElementAt(i).Count()) return false;
        }

        return true;
    }

    // whether this matrix contains the other matrix
    public bool SameStructure(IEnumerable<IEnumerable<T>> other)
    {
        if (other == null && _matrix==null) return _matrix==null;
        if(_matrix.Count!=other.Count()) return false;

        for(int i=0;i< _matrix.Count; i++)
        {
            if (other.ElementAt(i) == null && _matrix[i] != null)
                return false;
            if (_matrix.Count != other.ElementAt(i).Count()) return false;
        }

        return true;
    }

    public List<T> this[int i]
    {
        get { return _matrix[i].list; }
        set { _matrix[i].list = value; }
    }

    public bool Equals(IEnumerable<IEnumerable<T>> other)
    {
        return Equals(new Matrix<T>(other));
    }

    public override bool Equals(object obj)
    {
        Matrix<T> other = obj as Matrix<T>;
        if (other == null) return _matrix == null;

        if (other._matrix.Count != _matrix.Count) return false;
        for(int i=0;i< _matrix.Count; i++)
        {
            if (_matrix[i].list.Count!= other._matrix[i].list.Count) return false;
            for(int j = 0; j < _matrix[i].list.Count; j++)
            {
                if (!_matrix[i].list[j].Equals(other._matrix[i].list[j])) return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        return _matrix == null ? 0 : _matrix.Count+1;
    }
}
