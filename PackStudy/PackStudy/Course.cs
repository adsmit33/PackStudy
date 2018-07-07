namespace PackStudy
{
    public class Course
    {
        public string id;
        public string semester;
        public static Course MakeCourse(string id, string semester)
        {
            Course c = new Course();
            c.id = id;
            c.semester = semester;
            return c;
        }
    }
}