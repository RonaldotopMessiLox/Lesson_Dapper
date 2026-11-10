using Dapper;
using Npgsql;
using Models.Student;

await using var connection =
    new NpgsqlConnection("Host=wotacoma.ru;Port=5090;Username=ronaldotopmessilox;Password=79042732605;Database=ronaldotopmessilox");

try
{
    Console.WriteLine("Подключение к PostgreSQL успешно!");

    var students = await connection.QueryAsync<Student>(
        """
        SELECT "Id", "FirstName", "LastName", "Age"
        FROM "Students"
        """
        );

    int userChoise = 0;
    int affectedRows;
    int ar = 0;
    do
    {
        Console.WriteLine("1 - вывести всех студентов.\n2 - сортировать по фамилии.\n3 - Вывести колличество студентов\n4 - вывести количество старше указанного возраста\n5 - Поиск по айди\n6 - Поиск по фамилии\n7 - Поиск по возрасту\n8 - Создание студента\n9 - Обновить студента по айди\n10 - Удаление по айди\n11 - колличество затронутых записей\n0 - выход.");

        userChoise = Convert.ToInt32(Console.ReadLine());

        switch (userChoise)
        {

            //Все студенты
            case 1:
                Console.WriteLine("\n=== Список студентов ===");

                foreach (var student in students)
                {
                    Console.WriteLine(
                        $"{student.Id}: {student.FirstName} " +
                        $"{student.LastName}, возраст: {student.Age}");
                }
                break;

            // Сортировка по фамилии
            case 2:

                Console.WriteLine("\n=== Отсортированный по фамилии список студентов ===");

                var sortedStudents = await connection.QueryAsync<Student>(
                    """
                    SELECT "Id", "FirstName", "LastName", "Age"
                    FROM "Students"
                    ORDER BY "LastName"
                    """);

                foreach (var student in sortedStudents)
                {
                    Console.WriteLine(
                        $"{student.Id}: {student.FirstName} " +
                        $"{student.LastName}, возраст: {student.Age}");
                }
                break;

            // Колличество студентов
            case 3:
                Console.WriteLine($"\nКолличество студентов: {students.Count()}");
                break;

            // СТарше указанного возраста
            case 4:

                Console.Write("\nУкажите возраст для сравнения: ");

                int age = Convert.ToInt32(Console.ReadLine());

                var studentsOlder = await connection.QueryAsync<Student>(
                    """
                    SELECT "Id", "FirstName", "LastName", "Age"
                    FROM "Students"
                    WHERE "Age" > @Age
                    """, new { Age = age });

                Console.WriteLine($"\n=== Список студентов старше {age} лет ===");

                foreach (var student in studentsOlder)
                {
                    Console.WriteLine(
                        $"{student.Id}: {student.FirstName} " +
                        $"{student.LastName}, возраст: {student.Age}");
                }

                break;

            // Посик по айди
            case 5:
                Console.Write("Введите айди для поиска студентов: ");
                int id = Convert.ToInt32(Console.ReadLine());

                var searchedStudent = await connection.QuerySingleOrDefaultAsync<Student>(
                    """
                    SELECT "Id", "FirstName", "LastName", "Age"
                    FROM "Students"
                    WHERE "Id" = @Id
                    """, new { Id = id });

                if (searchedStudent != null)
                {
                    Console.WriteLine(
                        $"{searchedStudent.Id}: {searchedStudent.FirstName} " +
                        $"{searchedStudent.LastName}, возраст: {searchedStudent.Age}");
                }
                else Console.WriteLine("Студент не найден!");
                break;

            // Поиск по фамилии
            case 6:
                Console.Write("Введите фамилию для поиска студентов: ");
                string lastName = Console.ReadLine();

                var searchedStudent1 = await connection.QuerySingleOrDefaultAsync<Student>(
                    """
                    SELECT "Id", "FirstName", "LastName", "Age"
                    FROM "Students"
                    WHERE "LastName" = @LastName
                    """, new { LastName = lastName });

                if (searchedStudent1 != null)
                {
                    Console.WriteLine(
                        $"{searchedStudent1.Id}: {searchedStudent1.FirstName} " +
                        $"{searchedStudent1.LastName}, возраст: {searchedStudent1.Age}");
                }
                else Console.WriteLine("Студент не найден!");
                break;

            // Поиск по возрасту
            case 7:
                Console.Write("Введите возраст для поиска: ");
                int age1 = Convert.ToInt32(Console.ReadLine());
                var searchedStudent2 = await connection.QueryAsync<Student>(
                    """
                    SELECT "Id", "FirstName", "LastName", "Age"
                    FROM "Students"
                    WHERE "Age" = @Age
                    """, new { Age = age1 });

                foreach(var student in searchedStudent2)
                {
                    Console.WriteLine(
                        $"{student.Id}: {student.FirstName} " +
                        $"{student.LastName}, возраст: {student.Age}");
                }

                break;

            // Создание нового студенца
            case 8:
                Console.WriteLine("Введите параметры для нового студента: ");
                Console.Write("Имя: ");
                string name = Console.ReadLine()!;
                Console.Write("Фамилия: ");
                string lname = Console.ReadLine()!;
                Console.Write("Возраст: ");
                int age3 = Convert.ToInt32(Console.ReadLine());
                affectedRows = await connection.ExecuteAsync("""
                    INSERT INTO "Students" ("FirstName", "LastName", "Age", "GroupId")
                    VALUES (@FirstName, @LastName, @Age, @GroupId)
                    """, new { FirstName = name, LastName = lname, Age = age3, GroupId = 1 });
                ar++;
                break;

            // Обновление студенца
            case 9:
                Console.WriteLine("Введита айди: ");
                int id2 = Convert.ToInt32(Console.ReadLine());
                var currentSt = await connection.QuerySingleOrDefaultAsync<Student>(
                    """
                    SELECT "Id", "FirstName", "LastName", "Age"
                    FROM "Students"
                    WHERE "Id" = @Id
                    """, new { Id = id2 }
                    );
                if (currentSt != null)
                {
                    Console.WriteLine($"Введите новое имя (Текущее: {currentSt.FirstName})");
                    string name1 = Console.ReadLine()!;
                    Console.WriteLine($"Введите новую фамилию (Текущее: {currentSt.LastName})");
                    string lname1 = Console.ReadLine()!;
                    Console.WriteLine($"Введите новый возраст (Текущее: {currentSt.Age})");
                    int age2 = Convert.ToInt32(Console.ReadLine())!;

                    affectedRows = await connection.ExecuteAsync("""
                    UPDATE "Students"
                    SET "FirstName" = @FirstName, "LastName" = @LastName, "Age" = @Age
                    WHERE "Id" = @Id
                    """, new { FirstName = name1, LastName = lname1, Age = age2, Id = id2 });
                    if (affectedRows == 0) Console.WriteLine("Студент не найден!");
                    else Console.WriteLine("Успешно обновлено!"); students = await connection.QueryAsync<Student>(""" SELECT "Id", "FirstName", "LastName", "Age"FROM "Students" """); ar++;
                }
                else Console.WriteLine("Студент не найден!");
                break;

            //Удаление по айди
            case 10:
                Console.WriteLine("Введита айди: ");
                int id3 = Convert.ToInt32(Console.ReadLine());
                affectedRows = await connection.ExecuteAsync(
                    """
                    DELETE FROM "Students"
                    WHERE "Id" = @Id
                    """, new { Id = id3 });
                if (affectedRows == 0) Console.WriteLine("Студент не найден!");
                else Console.WriteLine("Успешно удалено!"); students = await connection.QueryAsync<Student>(""" SELECT "Id", "FirstName", "LastName", "Age"FROM "Students""");ar++;
                break;
            // Посмотреть количество затронутых записей 
            case 11:
                Console.WriteLine($"Колличество затронутых записей за сессию: {ar}");
                break;
            default:
                break;
        }
        Console.ReadLine();
        Console.Clear();
    } while (userChoise != 0);
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка работы с базой данных: {ex.Message}");
}