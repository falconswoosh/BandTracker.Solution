using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace UniversityRegistrar.Models
{
  public class Student
  {
    public string Name {get; private set;}
    public string EnrollmentDate {get; private set;}
    public int Id {get; private set;}

    public Student(string name, string enrollment, int id = 0)
    {
      Name = name;
      EnrollmentDate = enrollment;
      Id = id;
    }
    public override bool Equals(System.Object otherStudent)
    {
      if (!(otherStudent is Student))
      {
          return false;
      }
      else
      {
          Student newStudent = (Student) otherStudent;
          bool idEquality = (this.Id == newStudent.Id);
          bool nameEquality = (this.Name == newStudent.Name);
          bool enrollEquality = (this.EnrollmentDate == newStudent.EnrollmentDate);
          return (idEquality && nameEquality && enrollEquality);
      }
    }

    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"INSERT INTO students (name, enrollment_date) VALUES (@name, @enrollment);";

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@name";
      name.Value = Name;
      cmd.Parameters.Add(name);

      MySqlParameter enrollment = new MySqlParameter();
      enrollment.ParameterName = "@enrollment";
      enrollment.Value = EnrollmentDate;
      cmd.Parameters.Add(enrollment);

      cmd.ExecuteNonQuery();
      Id = (int) cmd.LastInsertedId;

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static List<Student> GetAll()
    {
      List<Student> roster = new List<Student>();

      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand();
      cmd.CommandText = @"SELECT * FROM students;";
      MySqlDataReader rdr = cmd.ExecuteReader();
      while(rdr.Read())
      {
        int studentId = rdr.GetInt32(0);
        string studentName = rdr.GetString(1);
        string studentEnrollmentDate = rdr.GetString(2);
        Student newStudent = new Student(studentName, studentEnrollmentDate, studentId);
        roster.Add(newStudent);
      }
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
      return roster;
    }

    public static void ClearAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM students; ALTER TABLE students AUTO_INCREMENT = 1; DELETE FROM students_courses; ALTER TABLE students_courses AUTO_INCREMENT = 1;";
      cmd.ExecuteNonQuery();

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
    public static void Delete(int searchId = 0)
    {
      if (searchId > 0)
      {
        MySqlConnection conn = DB.Connection();
        conn.Open();

        MySqlCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"DELETE FROM students WHERE id = @thisId; DELETE FROM students_courses WHERE student_id = @thisId;";

        MySqlParameter thisId = new MySqlParameter();
        thisId.ParameterName = "@thisId";
        thisId.Value = searchId;
        cmd.Parameters.Add(thisId);

        cmd.ExecuteNonQuery();

        conn.Close();
        if (conn != null)
        {
          conn.Dispose();
        }
      }
      return;
    }

    public static Student Find(int searchId = 0)
    {
      if (searchId > 0)
      {
        MySqlConnection conn = DB.Connection();
        conn.Open();

        MySqlCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT * FROM students WHERE id = @thisId;";

        MySqlParameter thisId = new MySqlParameter();
        thisId.ParameterName = "@thisId";
        thisId.Value = searchId;
        cmd.Parameters.Add(thisId);

        string studentName = "";
        string studentEnrollmentDate = "";

        var rdr = cmd.ExecuteReader() as MySqlDataReader;
        while(rdr.Read())
        {
          searchId = rdr.GetInt32(0);
          studentName = rdr.GetString(1);
          studentEnrollmentDate = rdr.GetString(2);
        }

        Student foundStudent = new Student(studentName, studentEnrollmentDate, searchId);

        conn.Close();
        if (conn != null)
        {
          conn.Dispose();
        }
        return foundStudent;
      }
      Student errStudent = new Student("ERR","ERR",0);
      return errStudent;
    }

    public void Update(string name="", string enrollmentDate="")
    {
      if(!String.IsNullOrEmpty(name))
      {this.Name = name;}
      if(!String.IsNullOrEmpty(enrollmentDate))
      {this.EnrollmentDate = enrollmentDate;}

      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = conn.CreateCommand();
      cmd.CommandText = @"SET @name, @enrollmentDate FROM students WHERE id = @thisId;";

      MySqlParameter thisName = new MySqlParameter();
      thisName.ParameterName = "@name";
      thisName.Value = this.Name;
      cmd.Parameters.Add(thisName);

      MySqlParameter thisEnrollmentDate = new MySqlParameter();
      thisEnrollmentDate.ParameterName = "@enrollmentDate";
      thisEnrollmentDate.Value = this.EnrollmentDate;
      cmd.Parameters.Add(thisEnrollmentDate);

        conn.Close();
        if (conn != null)
        {
          conn.Dispose();
        }
      return;
    }

    public void Register(int id)
    {
      if (id > 0)
      {
        MySqlConnection conn = DB.Connection();
        conn.Open();

        MySqlCommand cmd = conn.CreateCommand();
        cmd.CommandText  = @"INSERT INTO students_courses (student_id , course_id) VALUES (@studentId , @courseId);";

        MySqlParameter studentId = new MySqlParameter();
        studentId.ParameterName  = "@studentId";
        studentId.Value          = this.Id;
        cmd.Parameters.Add(studentId);

        MySqlParameter courseId = new MySqlParameter();
        courseId.ParameterName  = "@courseId";
        courseId.Value          = id;
        cmd.Parameters.Add(courseId);

        cmd.ExecuteNonQuery();

        conn.Close();
        if (conn!=null)
        {conn.Dispose();}
      }
    }
    public void RegisterCourseNumber(string crn)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      conn.Close();
      if (conn!=null)
      {conn.Dispose();}
    }
    public bool IsRegistered()
    {
      bool isTakingCourses = false;

      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand();
      cmd.CommandText = @"SELECT * FROM students_courses WHERE student_id = @studentId;";

      MySqlParameter studentId = new MySqlParameter();
      studentId.ParameterName = "@studentId";
      studentId.Value = this.Id;
      cmd.Parameters.Add(studentId);

      MySqlDataReader rdr = cmd.ExecuteReader();

        if (rdr.Read())
        { isTakingCourses = true; }

      conn.Close();
      if (conn!=null)
      {conn.Dispose();}

      return isTakingCourses;
    }

    public List<Course> GetSchedule()
    {
      List<Course> studentSchedule = new List<Course>();

      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand();
      cmd.CommandText = @"SELECT * FROM students_courses JOIN courses ON (students_courses.course_id) WHERE student_id = @studentId;";

      MySqlParameter studentId = new MySqlParameter();
      studentId.ParameterName = "@studentId";
      studentId.Value = this.Id;
      cmd.Parameters.Add(studentId);

      MySqlDataReader rdr = cmd.ExecuteReader();

      while(rdr.Read())
      {
        studentSchedule.Add(new Course(rdr.GetString(4), rdr.GetString(5), rdr.GetInt32(3)));
      }
      conn.Close();

      if (conn != null)
      {
        conn.Dispose();
      }
      return studentSchedule;
    }
  }
}
