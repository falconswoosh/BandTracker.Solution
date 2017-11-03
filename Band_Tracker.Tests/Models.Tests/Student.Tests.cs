using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniversityRegistrar.Models;

namespace UniversityRegistrar.Models.Tests
{
  [TestClass]
  public class StudentTests : IDisposable
  {
    public StudentTests()
    {
      DBConfiguration.ConnectionString = "server=localhost;user id=root;password=root;port=8889;database=university_registrar_test;";
    }

    public void Dispose()
    {
      Student.ClearAll();
      Course.ClearAll();
    }

    [TestMethod]
    public void ClearAll_ClearsAllStudentsFromDatabase_0()
    {
      //Arrange
      List<Student> testList = new List<Student>();
      Student studentA = new Student("Alex","test");
      Student studentB = new Student("Bob","test");
      Student studentC = new Student("Charlie","test");
      testList.Add(studentA);
      testList.Add(studentB);
      testList.Add(studentC);
      studentA.Save();
      studentB.Save();
      studentC.Save();

      //Act
      Student.ClearAll();
      List<Student> resultList = Student.GetAll();
      Assert.AreEqual(true,resultList.Count==0);
    }

    [TestMethod]
    public void Save_SaveStudent()
    {
      //Arrange
      Student testStudent = new Student("test-name","test-date");
      //Act
      testStudent.Save();

      Assert.AreEqual(true,Student.GetAll().Count==1);
    }

    [TestMethod]
    public void Find_FindsStudentInDatabase_Category()
    {
      Student testStudent = new Student("Ronnie", "Nov. 1, 2004");
      testStudent.Save();

      Student foundStudent = Student.Find(1);
      Assert.AreEqual(testStudent, foundStudent);
    }

    [TestMethod]
    public void Delete_DeleteStudentInDatabase_0()
    {
      Student testStudent = new Student("Ronnie", "Nov. 1, 2004");
      testStudent.Save();
      int newID = testStudent.Id;
      int numOfStudents = Student.GetAll().Count;
      Student.Delete(newID);
      Assert.AreEqual(false, Student.GetAll().Count == numOfStudents);
    }

    [TestMethod]
    public void Register_AddCourseToStudent_Student()
    {
      Course testCourse = new Course("History", "HIST101");
      testCourse.Save();

      Course testCourse2 = new Course("blah", "blah101");
      testCourse2.Save();

      Course testCourse3 = new Course("bleep", "bloop");
      testCourse3.Save();

      Student testStudent = new Student("Bob", "11/01/17");
      testStudent.Save();
      testStudent.Register(testCourse.Id);
      testStudent.Register(testCourse2.Id);

      Student testStudent2 = new Student("Betty", "09/29/17");
      testStudent2.Save();
      testStudent2.Register(testCourse.Id);

      List<Course> testList = testStudent.GetSchedule();
      List<Course> testlist2 = testStudent2.GetSchedule();

      CollectionAssert.AreNotEqual(testStudent.GetSchedule(), testStudent2.GetSchedule());
    }
  }
}
