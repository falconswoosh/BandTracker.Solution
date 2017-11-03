using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniversityRegistrar.Models;

namespace UniversityRegistrar.Models.Tests
{
  [TestClass]
  public class CourseTests : IDisposable
  {
    public CourseTests()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=university_registrar_test;";
    }
    public void Dispose()
    {
      Course.ClearAll();
      Student.ClearAll();
    }

    [TestMethod]
    public void Delete_DeleteCourseInDatabase_0()
    {
      Course testCourse = new Course("History of Snakes", "HST100");
      testCourse.Save();
      int newID = testCourse.Id;
      Course.Delete(newID);
      int allCourses = Course.GetAll().Count;
      Assert.AreEqual(allCourses, Course.GetAll().Count);
    }

    [TestMethod]
    public void ClearAll_ClearsAllCoursesFromDatabase_0()
    {
      List<Course> testList = new List<Course>();
      Course courseA = new Course("History of Snakes", "HST100");
      courseA.Save();
      Course courseB = new Course("History of Tacos", "HST101");
      courseB.Save();
      Course courseC = new Course("History of History", "HST102");
      courseC.Save();

      testList.Add(courseA);
      testList.Add(courseB);
      testList.Add(courseC);

      Course.ClearAll();
      List<Course> resultList = Course.GetAll();

      Assert.AreEqual(0,resultList.Count);
    }
    [TestMethod]
    public void Save_SaveCourse_()
    {
    Course testCourse = new Course("test-coursename", "test-coursenumber");
    testCourse.Save();
    Assert.AreEqual(true,Course.GetAll().Count==1);
    }

    [TestMethod]
    public void Find_FindsCourseInDatabase_Course()
    {
      Course testCourse = new Course("History", "HIST101");
      testCourse.Save();

      Course foundCourse = Course.Find(1);
      Assert.AreEqual(testCourse, foundCourse);
    }

    [TestMethod]
    public void Update_UpdateCourseInDatabase_Course()
    {
      Course testCourse = new Course("History", "HIST101");
      testCourse.Save();

      string newCourseName = "Math";
      string newCourseNumber = "MATH101";

      Course newCourse = new Course(newCourseName, newCourseNumber);
      testCourse.Update(newCourseName, newCourseNumber);

      Assert.AreEqual(testCourse.CourseName, newCourse.CourseName);
      Assert.AreEqual(testCourse.CourseNumber, newCourse.CourseNumber);
    }

    [TestMethod]
    public void GetClassRoster_FindStudentsTakingCourse()
    {
      Course courseA = new Course("History of Snakes", "HST100");
      courseA.Save();
      Course courseB = new Course("History of Tacos", "HST101");
      courseB.Save();
      Course courseC = new Course("History of History", "HST102");
      courseC.Save();
      Student studentA = new Student("Alex","test");
      studentA.Save();
      studentA.Register(courseA.Id);
      studentA.Register(courseC.Id);
      Student studentB = new Student("Bob","test");
      studentB.Save();
      studentB.Register(courseA.Id);
      studentB.Register(courseB.Id);
      Student studentC = new Student("Charlie","test");
      studentC.Save();

      Assert.AreEqual(studentA.IsRegistered(),studentB.IsRegistered());
      Assert.AreEqual(!studentA.IsRegistered(),studentC.IsRegistered());
      Assert.AreEqual(2,courseA.GetRoster().Count);
    }
  }
}
