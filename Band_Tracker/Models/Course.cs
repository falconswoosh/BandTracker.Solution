using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;


namespace UniversityRegistrar.Models
{
  public class Course
  {
    public string CourseName {get; private set;}
    public string CourseNumber {get; private set;}
    public int Id {get; private set;}

    public Course(string name, string number, int id = 0)
    {
      CourseName = name;
      CourseNumber = number;
      Id = id;
    }

    public static void Delete(int searchId = 0)
    {
      if (searchId > 0)
      {
        MySqlConnection conn = DB.Connection();
        conn.Open();

        MySqlCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"DELETE FROM courses WHERE id = @thisId; DELETE FROM students_courses WHERE course_id = @thisId;";

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
    }

    public void Update(string name="", string number="")
    {
      if(!String.IsNullOrEmpty(name))
      {
        this.CourseName = name;
      }
      if(!String.IsNullOrEmpty(number))
      {
        this.CourseNumber = number;
      }

      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = conn.CreateCommand();
      cmd.CommandText = @"SET @name, @number FROM courses where id = @thisId;";

      MySqlParameter thisname = new MySqlParameter();
      thisname.ParameterName = "@name";
      thisname.Value = this.CourseName;
      cmd.Parameters.Add(thisname);

      MySqlParameter thisnumber = new MySqlParameter();
      thisnumber.ParameterName = "@number";
      thisnumber.Value = this.CourseNumber;
      cmd.Parameters.Add(thisnumber);

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return;
    }

    public override bool Equals(System.Object otherCourse)
    {
      if (!(otherCourse is Course))
      {
        return false;
      }
      else
      {
        Course newCourse = (Course) otherCourse;
        bool idEquality = (this.Id == newCourse.Id);
        bool nameEquality = (this.CourseName == newCourse.CourseName);
        bool numberEquality = (this.CourseNumber == newCourse.CourseNumber);
        return (idEquality && nameEquality && numberEquality);
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
      cmd.CommandText = @"INSERT INTO courses (course_name, course_number) VALUES (@courseName, @courseNumber);";

      MySqlParameter name = new MySqlParameter();
      name.ParameterName = "@courseName";
      name.Value = this.CourseName;
      cmd.Parameters.Add(name);

      MySqlParameter number = new MySqlParameter();
      number.ParameterName = "@courseNumber";
      number.Value = this.CourseNumber;
      cmd.Parameters.Add(number);

      cmd.ExecuteNonQuery();
      Id = (int) cmd.LastInsertedId;

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static Course Find(int searchId = 0)
    {
      if (searchId > 0)
      {
        MySqlConnection conn = DB.Connection();
        conn.Open();

        MySqlCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"SELECT * FROM courses WHERE id = @thisId;";

        MySqlParameter thisId = new MySqlParameter();
        thisId.ParameterName = "@thisId";
        thisId.Value = searchId;
        cmd.Parameters.Add(thisId);

        string courseName = "";
        string courseNumber = "";

        var rdr = cmd.ExecuteReader() as MySqlDataReader;
        while(rdr.Read())
        {
          searchId = rdr.GetInt32(0);
          courseName = rdr.GetString(1);
          courseNumber = rdr.GetString(2);
        }

        Course foundCourse = new Course(courseName, courseNumber, searchId);

        conn.Close();
        if (conn != null)
        {
          conn.Dispose();
        }
        return foundCourse;
      }
      Course errCourse = new Course("ERR","ERR",0);
      return errCourse;
    }

    public static List<Course> GetAll()
    {
      List<Course> allcourses = new List<Course>();

      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand();
      cmd.CommandText = @"SELECT * FROM courses;";
      MySqlDataReader rdr = cmd.ExecuteReader();
      while(rdr.Read())
      {
        int courseId = rdr.GetInt32(0);
        string courseName = rdr.GetString(1);
        string courseNumber = rdr.GetString(2);

        Course newCourse = new Course(courseName, courseNumber, courseId);
        allcourses.Add(newCourse);
      }
      conn.Close();
      if(conn != null)
      {
        conn.Dispose();
      }
      return allcourses;
    }

    public static void ClearAll()
      {
        MySqlConnection conn = DB.Connection();
        conn.Open();

        var cmd = conn.CreateCommand() as MySqlCommand;
        cmd.CommandText = @"DELETE FROM courses; ALTER TABLE courses AUTO_INCREMENT = 1;DELETE FROM students_courses; ALTER TABLE students_courses AUTO_INCREMENT = 1;";
        cmd.ExecuteNonQuery();

        conn.Close();
        if (conn != null)
        {
          conn.Dispose();
        }
      }

    public List<Student> GetRoster()
    {
      List<Student> classRoster = new List<Student>();

      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = conn.CreateCommand();
      cmd.CommandText  = @"SELECT * FROM students_courses JOIN students  ON(students_courses.student_id=students.id) WHERE course_id = @thisCourse;";

      MySqlParameter thisCourse = new MySqlParameter();
      thisCourse.ParameterName  = "@thisCourse";
      thisCourse.Value          = this.Id;
      cmd.Parameters.Add(thisCourse);

      MySqlDataReader rdr = cmd.ExecuteReader();
      while(rdr.Read())
      {
        int studentId = rdr.GetInt32(3);
        string studentName = rdr.GetString(4);
        string studentEnrollmentDate = rdr.GetString(5);
        Student newStudent = new Student(studentName, studentEnrollmentDate, studentId);
        classRoster.Add(newStudent);
      }
      conn.Close();
      if (conn!=null)
      {conn.Dispose();}
      return classRoster;
    }
  }
}
