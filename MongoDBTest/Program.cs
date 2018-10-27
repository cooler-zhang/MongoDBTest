using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //CreateStudents();
            CreateIndex();

            GetStudents();
            FindStudent();
        }

        public static void DropCollection()
        {
            using (var ctx = new MongoDbContext("cooler"))
            {
                ctx.DropCollection<Student>();
            }
        }

        public static void CreateStudents()
        {
            using (var ctx = new MongoDbContext("cooler"))
            {
                var student = new Student("Jim", 10);
                student.Courses.Add(new Course("语文"));
                student.Courses.Add(new Course("数学"));
                student.Courses.Add(new Course("英语"));
                student.Courses.Add(new Course("音乐"));

                ctx.Create<Student>(student);

                var student2 = new Student("Tom", 10);
                student2.Courses.Add(new Course("语文"));
                student2.Courses.Add(new Course("数学"));
                student2.Courses.Add(new Course("英语"));
                student2.Courses.Add(new Course("体育"));

                ctx.Create<Student>(student2);
            }
        }

        public static void GetStudents()
        {
            using (var ctx = new MongoDbContext("cooler"))
            {
                var students = ctx.GetList<Student>();
            }
        }

        public static void FindStudent()
        {
            using (var ctx = new MongoDbContext("cooler"))
            {
                var student = ctx.Find<Student>("59e38325c5ca8029181a32ef");
            }
        }

        public static void CreateIndex()
        {
            using (var ctx = new MongoDbContext("cooler"))
            {
                var indexName = ctx.CreateIndex(Builders<Student>.IndexKeys.Ascending(a => a.Name), new CreateIndexOptions() { Unique = true });

                ctx.DropIndex<Student>(indexName);
            }
        }
    }

    public class Student : MongoBaseEntity
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public IList<Course> Courses { get; set; }

        public Student(string name, int age)
        {
            this.Name = name;
            this.Age = age;
            this.Courses = new List<Course>();
        }
    }

    public class Course
    {
        public string Name { get; set; }

        public Course(string name)
        {
            this.Name = name;
        }
    }
}
