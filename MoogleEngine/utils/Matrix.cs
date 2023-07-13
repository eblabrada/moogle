using System;
using System.Diagnostics;

namespace Matrices
{
  public class Matrix
  {
    public int rows, columns;
    public double[,] matrix;

    public Matrix(int _rows, int _columns)
    {
      this.rows = _rows;
      this.columns = _columns;
      this.matrix = new double[_rows, _columns];
    }

    public Matrix(int _rows, int _columns, double[,] _matrix)
    {
      this.rows = _rows;
      this.columns = _columns;
      this.matrix = new double[_rows, _columns];

      if (_matrix.GetLength(0) != _rows || _matrix.GetLength(1) != _columns)
      {
        throw new ArgumentException("The size of matrix not coincides with the specifications", nameof(_matrix));
      }

      for (int i = 0; i < _rows; i++)
      {
        for (int j = 0; j < _columns; j++)
        {
          this.matrix[i, j] = _matrix[i, j];
        }
      }
    }

    // return size of matrix rows x columns
    public (int, int) getSize()
    {
      return (rows, columns);
    }

    // return true if position (x, y) is inside of the matrix
    private bool inside(int x, int y)
    {
      return x >= 0 && x < rows && y >= 0 && y < columns;
    }

    // set position (x, y) of matrix to newValue
    // return false if (x, y) is offside of the matrix
    // otherwise, return true
    public bool setValue(int x, int y, int newValue)
    {
      if (inside(x, y) == false) return false;
      this.matrix[x, y] = newValue;
      return true;
    }

    public double getValue(int x, int y)
    {
      if (inside(x, y) == false)
      {
        throw new ArgumentException("This coordinate not exists");
      }
      return matrix[x, y];
    }

    public static Matrix operator *(Matrix a, double alpha)
    {
      for (int i = 0; i < a.rows; i++)
      {
        for (int j = 0; j < a.columns; j++)
        {
          a.matrix[i, j] *= alpha;
        }
      }
      return a;
    }

    public static Matrix operator +(Matrix a) => a;
    public static Matrix operator -(Matrix a) => a * -1;
    public static Matrix operator /(Matrix a, double alpha) => a * (1 / alpha);

    public static Matrix operator +(Matrix a, Matrix b)
    {
      if (a.rows != b.rows || a.columns != b.columns)
      {
        throw new ArgumentException("The number of rows and columns not are equals");
      }
      for (int i = 0; i < a.rows; i++)
      {
        for (int j = 0; j < a.columns; j++)
        {
          a.matrix[i, j] += b.matrix[i, j];
        }
      }
      return a;
    }

    public static Matrix operator -(Matrix a, Matrix b) => a + -b;
    public static Matrix operator *(Matrix a, Matrix b)
    {
      if (a.columns != b.rows)
      {
        throw new ArgumentException("Can't multiplicate this matrices");
      }

      int newRows = a.rows, newColums = b.columns;
      Matrix res = new Matrix(newRows, newColums);
      for (int i = 0; i < a.rows; i++)
      {
        for (int j = 0; j < a.columns; j++)
        {
          for (int k = 0; k < b.columns; k++)
          {
            res.matrix[i, k] += a.matrix[i, j] * b.matrix[j, k];
          }
        }
      }
      return res;
    }

    // operations over matrices
    // return (number of solutions, solution if any exists)
    // complexity: O(N^3)
    (long, double[]) gauss()
    {
      const double EPS = 1e-9;
      const long INF = (long)1e18;

      int[] where = new int[columns - 1];
      for (int i = 0; i < where.Length; i++)
      {
        where[i] = -1;
      }

      for (int col = 0, row = 0; col < columns - 1 && row < rows; col++)
      {
        int selected = row;
        for (int i = row; i < rows; i++)
        {
          if (Math.Abs(matrix[selected, col]) > Math.Abs(matrix[i, col]))
          {
            selected = i;
          }
        }

        if (Math.Abs(matrix[selected, col]) < EPS)
        {
          continue;
        }

        for (int i = col; i <= columns - 1; i++)
        {
          (matrix[selected, i], matrix[row, i]) = (matrix[row, i], matrix[selected, i]);
        }
        where[col] = row;

        for (int i = 0; i < rows; i++)
        {
          if (i != row)
          {
            double c = matrix[i, col] / matrix[row, col];
            for (int j = col; j <= columns - 1; j++)
            {
              matrix[i, j] -= matrix[row, j] * c;
            }
          }
        }

        row++;
      }

      double[] res = new double[columns - 1];
      for (int i = 0; i < where.Length; i++)
      {
        if (where[i] != -1)
        {
          res[i] = matrix[where[i], columns - 1] / matrix[where[i], i];
        }
      }

      for (int i = 0; i < rows; i++)
      {
        double sum = 0;
        for (int j = 0; j < columns - 1; j++)
        {
          sum += matrix[i, j] * res[j];
        }

        if (Math.Abs(sum - matrix[i, columns - 1]) > EPS)
        {
          return (0, new double[0]); // incompatible
        }
      }

      for (int i = 0; i < columns - 1; i++)
      {
        if (where[i] == -1)
        {
          return (INF, new double[0]); // indeterminate
        }
      }

      return (1, res);
    }
  }

  public class SquareMatrix : Matrix
  {
    public SquareMatrix(int N) : base(N, N) { }
    public SquareMatrix(int N, double[,] _matrix) : base(N, N, _matrix) { }

    // return determinant of the square matrix  
    public double determinant()
    {
      const double EPS = 1e-9;

      double res = 1;
      for (int i = 0; i < rows; i++)
      {
        int k = i;
        for (int j = i + 1; j < columns; j++)
        {
          if (Math.Abs(matrix[j, i]) > Math.Abs(matrix[k, i]))
          {
            k = j;
          }
        }

        if (Math.Abs(matrix[k, i]) < EPS)
        {
          return 0.0;
        }

        for (int j = 0; j < columns; j++)
        {
          (matrix[i, j], matrix[k, j]) = (matrix[k, j], matrix[i, j]);
        }

        if (i != k) res *= -1;

        res *= matrix[i, i];
        for (int j = i + 1; j < rows; j++)
        {
          matrix[i, j] /= matrix[i, i];
        }

        for (int j = 0; j < rows; j++)
        {
          if (i != j && Math.Abs(matrix[j, i]) > EPS)
          {
            for (int p = i + 1; p < rows; p++)
            {
              matrix[j, p] -= matrix[i, p] * matrix[j, i];
            }
          }
        }
      }
      return res;
    }
  }
}
