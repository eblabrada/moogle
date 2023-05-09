using System;
using System.Diagnostics;

public class Matrix {
  public int r, c;
  public double[,] d;
  
  public Matrix() {
    this.r = this.c = 0;
    this.d = new double[0, 0];
  }
  
  public Matrix(int _r, int _c) {
    this.r = _r; this.c = _c;
    this.d = new double[r, c];
  }
  
  public Matrix(double[,] a) {
    this.r = a.GetLength(0);
    this.c = a.GetLength(1);
    this.d = new double[r, c];
    for (int i = 0; i < r; i++) {
      for (int j = 0; j < c; j++) {
        this.d[i, j] = a[i, j];
      }
    }
  }
}

public static class MatrixOperations {
  public static Matrix id(int r) {
    Matrix res = new Matrix(r, r);
    for (int i = 0; i < r; i++) {
      res.d[i, i] = 1;
    }
    return res;
  }
  
  public static Matrix sum(Matrix a, Matrix b) {
    // se asume que las matrices tengan la misma dimension
    Debug.Assert(a.r == b.r && a.c == b.c);
    int n = a.r, m = a.c;
    Matrix res = new Matrix(n, m);
    for (int i = 0; i < n; i++) {
      for (int j = 0; j < m; j++) {
        res.d[i, j] = a.d[i, j] + b.d[i, j];
      }
    }
    return res;
  }
    
  public static Matrix mul(Matrix a, Matrix b) {
    // se asume que a.c == b.r
    Debug.Assert(a.c == b.r);
    int n = a.r, m = b.c;
    Matrix res = new Matrix(n, m);
    for (int i = 0; i < n; i++) {
      for (int j = 0; j < m; j++) {
        for (int k = 0; k < b.c; k++) {
          res.d[i, j] += a.d[i, k] * b.d[k, j];
        }
      }
    }
    return res;
  }
  
  public static Matrix mul(Matrix a, double scalar) {
    Matrix res = new Matrix(a.r, a.c);
    for (int i = 0; i < a.r; i++) {
      for (int j = 0; j < a.c; j++) {
        res.d[i, j] = scalar * a.d[i, j];
      }
    }
    return res;
  }
  
  public static Matrix subst(Matrix a, Matrix b) {
    return sum(a, mul(b, -1));
  }
  
}