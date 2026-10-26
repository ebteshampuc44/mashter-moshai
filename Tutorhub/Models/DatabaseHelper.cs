// 📁 Models/DatabaseHelper.cs
// লোকেশন: Tutorbub/Models/DatabaseHelper.cs

using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Tutorbub.Models;

namespace Tutorbub.Models
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Database")
                ?? throw new InvalidOperationException("Connection string 'Database' not found.");
        }

        // ============================================================
        // ===== USER RELATED METHODS =====
        // ============================================================

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            string query = @"
                SELECT ""Id"", ""UserName"", ""Password"", ""FullName"", 
                       ""Email"", ""CreatedAt"", ""LastLoginAt"", ""Role"", ""IsActive"",
                       ""MobileNumber"", ""ProfileImage"", ""Gender"", ""AgeRange"", 
                       ""PrimaryDeviceType"", ""YearsOfExperience"", ""AreaType"",
                       ""Country"", ""StreetAddress"", ""PermanentAddress"",
                       ""EducationLevel"", ""CurrentStudyStatus"", ""ExamDegreeTitle"",
                       ""InstitutionName"", ""PassingYear"", ""IsCSEStudent"",
                       ""CvLink"", ""GithubProfile"", ""PortfolioLink"", ""LinkedInProfile"", ""ProfileImageLink""
                FROM ""Users""
                ORDER BY ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(MapUser(reader));
                }
                return users;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all users: " + ex.Message);
            }
        }

        public User? AuthenticateUser(string username, string password)
        {
            string query = @"
                SELECT ""Id"", ""UserName"", ""Password"", ""FullName"", 
                       ""Email"", ""CreatedAt"", ""LastLoginAt"", ""Role"", ""IsActive"",
                       ""MobileNumber"", ""ProfileImage"", ""Gender"", ""AgeRange"", 
                       ""PrimaryDeviceType"", ""YearsOfExperience"", ""AreaType"",
                       ""Country"", ""StreetAddress"", ""PermanentAddress"",
                       ""EducationLevel"", ""CurrentStudyStatus"", ""ExamDegreeTitle"",
                       ""InstitutionName"", ""PassingYear"", ""IsCSEStudent"",
                       ""CvLink"", ""GithubProfile"", ""PortfolioLink"", ""LinkedInProfile"", ""ProfileImageLink""
                FROM ""Users""
                WHERE LOWER(""UserName"") = LOWER(@username) 
                  AND ""Password"" = @password 
                  AND ""IsActive"" = TRUE";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                connection.Open();

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return MapUser(reader);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error authenticating user: " + ex.Message);
            }
        }

        public bool UpdateUserProfile(User user)
        {
            string query = @"
                UPDATE ""Users"" SET 
                    ""FullName"" = @fullName,
                    ""Email"" = @email,
                    ""MobileNumber"" = @mobileNumber,
                    ""Gender"" = @gender,
                    ""AgeRange"" = @ageRange,
                    ""PrimaryDeviceType"" = @primaryDeviceType,
                    ""YearsOfExperience"" = @yearsOfExperience,
                    ""AreaType"" = @areaType,
                    ""Country"" = @country,
                    ""StreetAddress"" = @streetAddress,
                    ""PermanentAddress"" = @permanentAddress,
                    ""EducationLevel"" = @educationLevel,
                    ""CurrentStudyStatus"" = @currentStudyStatus,
                    ""ExamDegreeTitle"" = @examDegreeTitle,
                    ""InstitutionName"" = @institutionName,
                    ""PassingYear"" = @passingYear,
                    ""IsCSEStudent"" = @isCSEStudent,
                    ""CvLink"" = @cvLink,
                    ""GithubProfile"" = @githubProfile,
                    ""PortfolioLink"" = @portfolioLink,
                    ""LinkedInProfile"" = @linkedInProfile,
                    ""ProfileImageLink"" = @profileImageLink
                WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@id", user.Id);
                command.Parameters.AddWithValue("@fullName", user.FullName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@email", user.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@mobileNumber", user.MobileNumber ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@gender", user.Gender ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ageRange", user.AgeRange ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@primaryDeviceType", user.PrimaryDeviceType ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@yearsOfExperience", user.YearsOfExperience ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@areaType", user.AreaType ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@country", user.Country ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@streetAddress", user.StreetAddress ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@permanentAddress", user.PermanentAddress ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@educationLevel", user.EducationLevel ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@currentStudyStatus", user.CurrentStudyStatus ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@examDegreeTitle", user.ExamDegreeTitle ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@institutionName", user.InstitutionName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@passingYear", user.PassingYear ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@isCSEStudent", user.IsCSEStudent ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@cvLink", user.CvLink ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@githubProfile", user.GithubProfile ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@portfolioLink", user.PortfolioLink ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@linkedInProfile", user.LinkedInProfile ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@profileImageLink", user.ProfileImageLink ?? (object)DBNull.Value);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user profile: " + ex.Message);
            }
        }

        public User? GetUserById(int userId)
        {
            string query = @"
                SELECT ""Id"", ""UserName"", ""Password"", ""FullName"", 
                       ""Email"", ""CreatedAt"", ""LastLoginAt"", ""Role"", ""IsActive"",
                       ""MobileNumber"", ""ProfileImage"", ""Gender"", ""AgeRange"", 
                       ""PrimaryDeviceType"", ""YearsOfExperience"", ""AreaType"",
                       ""Country"", ""StreetAddress"", ""PermanentAddress"",
                       ""EducationLevel"", ""CurrentStudyStatus"", ""ExamDegreeTitle"",
                       ""InstitutionName"", ""PassingYear"", ""IsCSEStudent"",
                       ""CvLink"", ""GithubProfile"", ""PortfolioLink"", ""LinkedInProfile"", ""ProfileImageLink""
                FROM ""Users""
                WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", userId);
                connection.Open();

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return MapUser(reader);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting user by ID: " + ex.Message);
            }
        }

        public bool RegisterUser(User user)
        {
            var trimmedEmail = (user.Email ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(trimmedEmail) ||
                !trimmedEmail.EndsWith("@gmail.com"))
            {
                throw new Exception("Email must end with @gmail.com (all lowercase).");
            }

            var gmailRegex = new Regex(@"^[A-Za-z0-9._%+-]+@gmail\.com$");
            if (!gmailRegex.IsMatch(trimmedEmail))
            {
                throw new Exception("Invalid Gmail address format.");
            }

            string query = @"
                INSERT INTO ""Users"" (""UserName"", ""Password"", ""FullName"", ""Email"", ""Role"")
                VALUES (@username, @password, @fullname, @email, 'User')";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", user.UserName);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@fullname", user.FullName);
                command.Parameters.AddWithValue("@email", trimmedEmail);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error registering user: " + ex.Message);
            }
        }

        public bool DeleteUser(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                using (var deleteRequestsCommand = new NpgsqlCommand(
                    "DELETE FROM \"TeacherRequests\" WHERE \"UserId\" = @id", connection, transaction))
                {
                    deleteRequestsCommand.Parameters.AddWithValue("@id", userId);
                    deleteRequestsCommand.ExecuteNonQuery();
                }

                using var deleteUserCommand = new NpgsqlCommand(
                    "DELETE FROM \"Users\" WHERE \"Id\" = @id", connection, transaction);
                deleteUserCommand.Parameters.AddWithValue("@id", userId);
                int rowsAffected = deleteUserCommand.ExecuteNonQuery();

                transaction.Commit();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error deleting user: " + ex.Message);
            }
        }

        public bool UpdatePassword(int userId, string newPassword)
        {
            string query = "UPDATE \"Users\" SET \"Password\" = @password WHERE \"Id\" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@password", newPassword);
                command.Parameters.AddWithValue("@id", userId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating password: " + ex.Message);
            }
        }

        public bool ToggleUserStatus(int userId, bool isActive)
        {
            string query = "UPDATE \"Users\" SET \"IsActive\" = @isActive WHERE \"Id\" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@isActive", isActive);
                command.Parameters.AddWithValue("@id", userId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error toggling user status: " + ex.Message);
            }
        }

        public bool UsernameExists(string username)
        {
            string query = "SELECT COUNT(*) FROM \"Users\" WHERE LOWER(\"UserName\") = LOWER(@username)";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", username);
                connection.Open();
                return Convert.ToInt64(command.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking username: " + ex.Message);
            }
        }

        public bool EmailExists(string email)
        {
            var trimmed = (email ?? string.Empty).Trim();

            string query = "SELECT COUNT(*) FROM \"Users\" WHERE LOWER(\"Email\") = LOWER(@email)";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@email", trimmed);
                connection.Open();
                return Convert.ToInt64(command.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking email: " + ex.Message);
            }
        }

        public void UpdateLastLogin(string username)
        {
            string query = @"UPDATE ""Users"" SET ""LastLoginAt"" = @lastLogin WHERE ""UserName"" = @username";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@lastLogin", DateTime.UtcNow);
                command.Parameters.AddWithValue("@username", username);
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating last login: " + ex.Message);
            }
        }

        // ✅ নতুন: সব টিচারের লিস্ট আনা
        public List<User> GetAllTeachers()
        {
            var list = new List<User>();
            string query = @"
                SELECT ""Id"", ""UserName"", ""Password"", ""FullName"", 
                       ""Email"", ""CreatedAt"", ""LastLoginAt"", ""Role"", ""IsActive"",
                       ""MobileNumber"", ""ProfileImage"", ""Gender"", ""AgeRange"", 
                       ""PrimaryDeviceType"", ""YearsOfExperience"", ""AreaType"",
                       ""Country"", ""StreetAddress"", ""PermanentAddress"",
                       ""EducationLevel"", ""CurrentStudyStatus"", ""ExamDegreeTitle"",
                       ""InstitutionName"", ""PassingYear"", ""IsCSEStudent"",
                       ""CvLink"", ""GithubProfile"", ""PortfolioLink"", ""LinkedInProfile"", ""ProfileImageLink""
                FROM ""Users"" WHERE ""Role"" = 'Teacher' ORDER BY ""Id"" DESC";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapUser(reader));
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetAllTeachers: " + ex.Message);
                return list;
            }
        }

        // ============================================================
        // ===== TEACHER REQUEST RELATED METHODS =====
        // ============================================================

        public bool CreateTeacherRequest(TeacherRequest request, out string? errorMessage)
        {
            errorMessage = null;
            string query = @"
                INSERT INTO ""TeacherRequests"" 
                (""UserId"", ""FullName"", ""Email"", ""MobileNumber"", ""Education"", ""Institution"", 
                 ""SubjectExpertise"", ""Experience"", ""TeachingStyle"", ""AvailableDays"", ""PreferredTime"", 
                 ""HourlyRate"", ""CvLink"", ""WhyTeach"", ""Status"", ""RequestDate"")
                VALUES 
                (@userId, @fullName, @email, @mobileNumber, @education, @institution, 
                 @subjectExpertise, @experience, @teachingStyle, @availableDays, @preferredTime, 
                 @hourlyRate, @cvLink, @whyTeach, 'Pending', @requestDate)";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@userId", request.UserId);
                command.Parameters.AddWithValue("@fullName", request.FullName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@email", request.Email ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@mobileNumber", request.MobileNumber ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@education", request.Education ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@institution", request.Institution ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@subjectExpertise", request.SubjectExpertise ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@experience", request.Experience ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@teachingStyle", request.TeachingStyle ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@availableDays", request.AvailableDays ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@preferredTime", request.PreferredTime ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@hourlyRate", request.HourlyRate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@cvLink", request.CvLink ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@whyTeach", request.WhyTeach ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@requestDate", request.RequestDate);

                connection.Open();
                int result = command.ExecuteNonQuery();
                return result > 0;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public string GetUserTeacherRequestStatus(int userId)
        {
            string query = @"SELECT ""Status"" FROM ""TeacherRequests"" 
                             WHERE ""UserId"" = @userId 
                             ORDER BY ""RequestDate"" DESC LIMIT 1";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();

                var result = command.ExecuteScalar();
                return result?.ToString() ?? "None";
            }
            catch
            {
                return "None";
            }
        }

        public List<TeacherRequest> GetAllTeacherRequests()
        {
            var requests = new List<TeacherRequest>();
            string query = @"SELECT * FROM ""TeacherRequests"" ORDER BY ""RequestDate"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    requests.Add(MapTeacherRequest(reader));
                }
                return requests;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting teacher requests: " + ex.Message);
            }
        }

        public bool UpdateTeacherRequest(int requestId, string status, string? adminNote)
        {
            string query = @"UPDATE ""TeacherRequests"" 
                             SET ""Status"" = @status, ""ResponseDate"" = @responseDate, ""AdminNote"" = @adminNote
                             WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@responseDate", DateTime.UtcNow);
                command.Parameters.AddWithValue("@adminNote", adminNote ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@id", requestId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating teacher request: " + ex.Message);
            }
        }

        public TeacherRequest? GetTeacherRequestById(int requestId)
        {
            string query = @"SELECT * FROM ""TeacherRequests"" WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", requestId);
                connection.Open();
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return MapTeacherRequest(reader);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting teacher request: " + ex.Message);
            }
        }

        public bool MakeUserTeacher(int userId)
        {
            string query = @"UPDATE ""Users"" SET ""Role"" = 'Teacher' WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", userId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error making user teacher: " + ex.Message);
            }
        }

        public bool DeleteTeacherRequestById(int requestId)
        {
            string query = "DELETE FROM \"TeacherRequests\" WHERE \"Id\" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", requestId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting teacher request: " + ex.Message);
            }
        }

        // ============================================================
        // ===== FIND TUTOR — APPROVED TEACHERS =====
        // ============================================================
        public List<TutorProfile> GetApprovedTutors()
        {
            var tutors = new List<TutorProfile>();

            string query = @"
                SELECT 
                    tr.""Id"", tr.""UserId"", tr.""FullName"", tr.""Email"", tr.""MobileNumber"",
                    tr.""Education"", tr.""Institution"", tr.""SubjectExpertise"", tr.""Experience"",
                    tr.""TeachingStyle"", tr.""AvailableDays"", tr.""PreferredTime"", tr.""HourlyRate"",
                    tr.""CvLink"", tr.""WhyTeach"", tr.""Status"", tr.""RequestDate"",
                    u.""ProfileImageLink""
                FROM ""TeacherRequests"" tr
                LEFT JOIN ""Users"" u ON tr.""UserId"" = u.""Id""
                WHERE tr.""Status"" = 'Approved'
                ORDER BY tr.""RequestDate"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();

                using var reader = command.ExecuteReader();
                int index = 1;

                while (reader.Read())
                {
                    var fullName = reader["FullName"]?.ToString() ?? "Tutor";
                    var requestId = reader.GetInt32(reader.GetOrdinal("Id"));
                    var tutorId = "TCH-" + (1000 + requestId);

                    var words = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string initials = "T";
                    if (words.Length >= 2)
                        initials = words[0][0].ToString().ToUpper() + words[1][0].ToString().ToUpper();
                    else if (words.Length == 1 && words[0].Length > 0)
                        initials = words[0][0].ToString().ToUpper();

                    var subjectExpertise = reader["SubjectExpertise"]?.ToString() ?? "";
                    var subjects = subjectExpertise
                        .Split(new[] { ',', '/', '&' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .ToList();
                    if (subjects.Count == 0) subjects.Add("General");

                    var classes = new List<string> { "All Classes" };

                    int expYears = 0;
                    var expStr = reader["Experience"]?.ToString() ?? "";
                    var expDigits = new string(expStr.Where(char.IsDigit).ToArray());
                    if (!string.IsNullOrEmpty(expDigits))
                    {
                        var numStr = expDigits.Length > 2 ? expDigits.Substring(0, 2) : expDigits;
                        int.TryParse(numStr, out expYears);
                    }

                    double rating = Math.Round(4.5 + (index % 5) * 0.1, 1);
                    int reviews = 20 + index * 3;

                    decimal monthlyFee = 5000 + (index % 6) * 1000;
                    var hourlyRateStr = reader["HourlyRate"]?.ToString() ?? "";
                    if (decimal.TryParse(hourlyRateStr, out decimal hourlyRate) && hourlyRate > 0)
                    {
                        monthlyFee = Math.Round(hourlyRate * 30, 0);
                    }

                    var photoUrl = "";
                    try
                    {
                        var imgIdx = reader.GetOrdinal("ProfileImageLink");
                        if (!reader.IsDBNull(imgIdx))
                        {
                            photoUrl = reader.GetString(imgIdx);
                        }
                    }
                    catch { }

                    tutors.Add(new TutorProfile
                    {
                        Id = requestId,
                        TutorId = tutorId,
                        FullName = fullName,
                        Initials = initials,
                        Gender = "Any",
                        PhotoUrl = photoUrl,
                        Subjects = subjects,
                        Classes = classes,
                        Qualification = reader["Education"]?.ToString() ?? "Not specified",
                        Institution = reader["Institution"]?.ToString() ?? "Not specified",
                        ExperienceYears = expYears,
                        Location = "Bangladesh",
                        TeachingMode = "Both",
                        MonthlyFee = monthlyFee,
                        Rating = rating,
                        TotalReviews = reviews,
                        AvailableTime = !string.IsNullOrEmpty(reader["PreferredTime"]?.ToString())
                            ? reader["PreferredTime"].ToString()!
                            : (reader["AvailableDays"]?.ToString() ?? "Contact for schedule"),
                        About = !string.IsNullOrEmpty(reader["WhyTeach"]?.ToString())
                            ? reader["WhyTeach"].ToString()!
                            : (!string.IsNullOrEmpty(reader["TeachingStyle"]?.ToString())
                                ? reader["TeachingStyle"].ToString()!
                                : "Experienced tutor ready to help students excel."),
                        IsVerified = true,
                        IsTopRated = index <= 3
                    });

                    index++;
                }

                return tutors;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting approved tutors: " + ex.Message);
                return new List<TutorProfile>();
            }
        }

        // ============================================================
        // ===== ✅ TUTOR REQUESTS (Student → Admin/Teacher) =====
        // ============================================================

        public bool CreateTutorRequest(TutorRequest req, out string? errorMessage)
        {
            errorMessage = null;
            string query = @"
                INSERT INTO ""TutorRequests"" 
                (""TutorId"", ""TutorName"", ""StudentName"", ""PhoneNumber"", ""WhatsAppNumber"",
                 ""Email"", ""StudentClass"", ""RequiredSubject"", ""PreferredArea"", ""TeachingMode"",
                 ""PreferredDays"", ""PreferredTime"", ""MonthlyBudget"", ""AdditionalNotes"",
                 ""RequestType"", ""RequestId"", ""Status"", ""CreatedAt"")
                VALUES 
                (@tutorId, @tutorName, @studentName, @phoneNumber, @whatsAppNumber,
                 @email, @studentClass, @requiredSubject, @preferredArea, @teachingMode,
                 @preferredDays, @preferredTime, @monthlyBudget, @additionalNotes,
                 @requestType, @requestId, 'Pending', @createdAt)
                RETURNING ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@tutorId", (object?)req.TutorId ?? DBNull.Value);
                command.Parameters.AddWithValue("@tutorName", (object?)req.TutorName ?? DBNull.Value);
                command.Parameters.AddWithValue("@studentName", req.StudentName ?? "");
                command.Parameters.AddWithValue("@phoneNumber", (object?)req.PhoneNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@whatsAppNumber", (object?)req.WhatsAppNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@email", (object?)req.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@studentClass", (object?)req.StudentClass ?? DBNull.Value);
                command.Parameters.AddWithValue("@requiredSubject", (object?)req.RequiredSubject ?? DBNull.Value);
                command.Parameters.AddWithValue("@preferredArea", (object?)req.PreferredArea ?? DBNull.Value);
                command.Parameters.AddWithValue("@teachingMode", (object?)req.TeachingMode ?? DBNull.Value);
                command.Parameters.AddWithValue("@preferredDays", (object?)req.PreferredDays ?? DBNull.Value);
                command.Parameters.AddWithValue("@preferredTime", (object?)req.PreferredTime ?? DBNull.Value);
                command.Parameters.AddWithValue("@monthlyBudget", (object?)req.MonthlyBudget ?? DBNull.Value);
                command.Parameters.AddWithValue("@additionalNotes", (object?)req.AdditionalNotes ?? DBNull.Value);
                command.Parameters.AddWithValue("@requestType", req.RequestType ?? "SpecificTutor");
                command.Parameters.AddWithValue("@requestId", (object?)req.RequestId ?? DBNull.Value);
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    req.Id = newId;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                Console.WriteLine("Error CreateTutorRequest: " + ex.Message);
                return false;
            }
        }

        public List<TutorRequest> GetAllTutorRequests(string? statusFilter = null)
        {
            var list = new List<TutorRequest>();
            string query = @"SELECT * FROM ""TutorRequests""";

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "all")
                query += @" WHERE ""Status"" = @status";

            query += @" ORDER BY ""CreatedAt"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "all")
                    command.Parameters.AddWithValue("@status", statusFilter);

                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapTutorRequest(reader));
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetAllTutorRequests: " + ex.Message);
                return list;
            }
        }

        // ✅ নতুন মেথড: টিচারের কাছে আসা Direct Requests
        public List<TutorRequest> GetTeacherDirectRequests(string tutorName)
        {
            var list = new List<TutorRequest>();
            string query = @"SELECT * FROM ""TutorRequests"" WHERE ""TutorName"" = @tutorName ORDER BY ""CreatedAt"" DESC";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@tutorName", tutorName);
                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(MapTutorRequest(reader));
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetTeacherDirectRequests: " + ex.Message);
                return list;
            }
        }

        public TutorRequest? GetTutorRequestById(int id)
        {
            string query = @"SELECT * FROM ""TutorRequests"" WHERE ""Id"" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                using var reader = command.ExecuteReader();
                if (reader.Read()) return MapTutorRequest(reader);
                return null;
            }
            catch { return null; }
        }

        public bool UpdateTutorRequestStatus(int id, string status, string? adminNote)
        {
            string query = @"
                UPDATE ""TutorRequests""
                SET ""Status"" = @status, ""AdminNote"" = @adminNote, ""UpdatedAt"" = @updatedAt
                WHERE ""Id"" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@status", status ?? "Pending");
                command.Parameters.AddWithValue("@adminNote", (object?)adminNote ?? DBNull.Value);
                command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error UpdateTutorRequestStatus: " + ex.Message);
                return false;
            }
        }

        public bool DeleteTutorRequest(int id)
        {
            string query = @"DELETE FROM ""TutorRequests"" WHERE ""Id"" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch { return false; }
        }

        public (int Total, int Pending, int Contacted, int Completed) GetTutorRequestStats()
        {
            string query = @"
                SELECT 
                    COUNT(*) AS total,
                    COUNT(*) FILTER (WHERE ""Status"" = 'Pending') AS pending,
                    COUNT(*) FILTER (WHERE ""Status"" = 'Contacted') AS contacted,
                    COUNT(*) FILTER (WHERE ""Status"" = 'Completed') AS completed
                FROM ""TutorRequests""";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return (
                        Convert.ToInt32(reader["total"]),
                        Convert.ToInt32(reader["pending"]),
                        Convert.ToInt32(reader["contacted"]),
                        Convert.ToInt32(reader["completed"])
                    );
                }
                return (0, 0, 0, 0);
            }
            catch
            {
                return (0, 0, 0, 0);
            }
        }

        // ============================================================
        // ===== ✅ TUITION APPLICATIONS FOR A SPECIFIC USER =====
        // ============================================================
        public List<TuitionApplication> GetUserTuitionApplications(int userId)
        {
            var list = new List<TuitionApplication>();

            string query = @"
                SELECT a.""Id"", a.""TuitionPostId"", a.""UserId"", a.""ApplicantName"",
                       a.""PhoneNumber"", a.""WhatsAppNumber"", a.""Email"", a.""CvLink"",
                       a.""CoverNote"", a.""Status"", a.""AdminNote"",
                       a.""AppliedAt"", a.""UpdatedAt"",
                       p.""Title"" AS PostTitle, p.""TuitionCode"", p.""Area"",
                       p.""Subjects"", p.""StudentClass"", p.""Salary"", p.""Status"" AS PostStatus
                FROM ""TuitionApplications"" a
                LEFT JOIN ""TuitionPosts"" p ON a.""TuitionPostId"" = p.""Id""
                WHERE a.""UserId"" = @userId
                ORDER BY a.""AppliedAt"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var app = new TuitionApplication
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        TuitionPostId = reader.GetInt32(reader.GetOrdinal("TuitionPostId")),
                        UserId = reader["UserId"] != DBNull.Value
                                    ? Convert.ToInt32(reader["UserId"]) : (int?)null,
                        ApplicantName = reader["ApplicantName"]?.ToString() ?? "",
                        PhoneNumber = reader["PhoneNumber"]?.ToString() ?? "",
                        WhatsAppNumber = reader["WhatsAppNumber"]?.ToString(),
                        Email = reader["Email"]?.ToString(),
                        CvLink = reader["CvLink"]?.ToString() ?? "",
                        CoverNote = reader["CoverNote"]?.ToString(),
                        Status = reader["Status"]?.ToString() ?? "Pending",
                        AdminNote = reader["AdminNote"]?.ToString(),
                        AppliedAt = reader["AppliedAt"] as DateTime? ?? DateTime.UtcNow,
                        UpdatedAt = reader["UpdatedAt"] as DateTime?
                    };

                    if (reader["PostTitle"] != DBNull.Value)
                    {
                        app.TuitionPost = new TuitionPost
                        {
                            Id = app.TuitionPostId,
                            Title = reader["PostTitle"]?.ToString() ?? "",
                            TuitionCode = reader["TuitionCode"]?.ToString(),
                            Area = reader["Area"]?.ToString(),
                            Subjects = reader["Subjects"]?.ToString(),
                            StudentClass = reader["StudentClass"]?.ToString(),
                            Salary = reader["Salary"]?.ToString(),
                            Status = reader["PostStatus"]?.ToString() ?? "Open"
                        };
                    }

                    list.Add(app);
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetUserTuitionApplications: " + ex.Message);
                return list;
            }
        }

        // ============================================================
        // ===== ✅ TEACHER DASHBOARD STATS (NEW) =====
        // ============================================================

        public TeacherDashboardStats GetTeacherDashboardStats(int userId)
        {
            var stats = new TeacherDashboardStats();

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using (var cmd = new NpgsqlCommand(
                    @"SELECT COUNT(*) FROM ""TeachWantedPosts"" WHERE ""UserId"" = @uid", connection))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    stats.TotalPosts = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (var cmd = new NpgsqlCommand(
                    @"SELECT COUNT(*) FROM ""TeachWantedComments"" c
                      INNER JOIN ""TeachWantedPosts"" p ON c.""PostId"" = p.""Id""
                      WHERE p.""UserId"" = @uid", connection))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    stats.TotalComments = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (var cmd = new NpgsqlCommand(
                    @"SELECT COALESCE(SUM(""LikeCount""), 0) FROM ""TeachWantedPosts"" 
                      WHERE ""UserId"" = @uid", connection))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    stats.TotalLikes = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (var cmd = new NpgsqlCommand(
                    @"SELECT COUNT(*) FROM ""TeachWantedComments"" c
                      INNER JOIN ""TeachWantedPosts"" p ON c.""PostId"" = p.""Id""
                      WHERE p.""UserId"" = @uid AND c.""ParentId"" IS NOT NULL", connection))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    stats.TotalReplies = Convert.ToInt32(cmd.ExecuteScalar());
                }

                using (var cmd = new NpgsqlCommand(
                    @"SELECT COUNT(*) FROM ""TuitionApplications"" WHERE ""UserId"" = @uid", connection))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    stats.TotalTuitionApplications = Convert.ToInt32(cmd.ExecuteScalar());
                }

                stats.TotalResponses = stats.TotalComments + stats.TotalLikes;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetTeacherDashboardStats error: " + ex.Message);
            }

            return stats;
        }

        public List<TeacherPostStat> GetTeacherPostStats(int userId)
        {
            var list = new List<TeacherPostStat>();

            string query = @"
                SELECT p.""Id"", p.""Content"", p.""ImageUrl"", p.""LikeCount"", p.""CreatedAt"",
                       (SELECT COUNT(*) FROM ""TeachWantedComments"" WHERE ""PostId"" = p.""Id"") AS ""CommentCount""
                FROM ""TeachWantedPosts"" p
                WHERE p.""UserId"" = @uid
                ORDER BY p.""CreatedAt"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@uid", userId);
                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new TeacherPostStat
                    {
                        PostId = reader.GetInt32(reader.GetOrdinal("Id")),
                        Content = reader["Content"]?.ToString() ?? "",
                        ImageUrl = reader["ImageUrl"]?.ToString(),
                        LikeCount = Convert.ToInt32(reader["LikeCount"]),
                        CommentCount = Convert.ToInt32(reader["CommentCount"]),
                        CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetTeacherPostStats error: " + ex.Message);
            }

            return list;
        }

        // ============================================================
        // ===== NOTIFICATION RELATED METHODS =====
        // ============================================================

        public bool CreateNotification(int userId, string title, string message,
            string type = "info", string? link = null, string icon = "fa-bell")
        {
            string query = @"
                INSERT INTO ""Notifications"" 
                (""UserId"", ""Title"", ""Message"", ""Type"", ""Icon"", ""Link"", ""IsRead"", ""CreatedAt"")
                VALUES 
                (@userId, @title, @message, @type, @icon, @link, FALSE, @createdAt)";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@title", title ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@message", message ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@type", type ?? "info");
                command.Parameters.AddWithValue("@icon", icon ?? "fa-bell");
                command.Parameters.AddWithValue("@link", link ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating notification: " + ex.Message);
                return false;
            }
        }

        public List<Notification> GetUserNotifications(int userId, int limit = 20)
        {
            var notifications = new List<Notification>();
            string query = @"
                SELECT ""Id"", ""UserId"", ""Title"", ""Message"", ""Type"", ""Icon"", 
                       ""Link"", ""IsRead"", ""CreatedAt""
                FROM ""Notifications""
                WHERE ""UserId"" = @userId
                ORDER BY ""CreatedAt"" DESC
                LIMIT @limit";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@limit", limit);
                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    notifications.Add(new Notification
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        Title = reader["Title"]?.ToString() ?? "",
                        Message = reader["Message"]?.ToString() ?? "",
                        Type = reader["Type"]?.ToString() ?? "info",
                        Icon = reader["Icon"]?.ToString() ?? "fa-bell",
                        Link = reader["Link"]?.ToString(),
                        IsRead = reader["IsRead"] as bool? ?? false,
                        CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow
                    });
                }
                return notifications;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting notifications: " + ex.Message);
                return new List<Notification>();
            }
        }

        public int GetUnreadNotificationCount(int userId)
        {
            string query = @"SELECT COUNT(*) FROM ""Notifications"" 
                             WHERE ""UserId"" = @userId AND ""IsRead"" = FALSE";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting unread count: " + ex.Message);
                return 0;
            }
        }

        public bool MarkNotificationAsRead(int notificationId, int userId)
        {
            string query = @"UPDATE ""Notifications"" 
                             SET ""IsRead"" = TRUE 
                             WHERE ""Id"" = @id AND ""UserId"" = @userId";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", notificationId);
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error marking as read: " + ex.Message);
                return false;
            }
        }

        public bool MarkAllNotificationsAsRead(int userId)
        {
            string query = @"UPDATE ""Notifications"" 
                             SET ""IsRead"" = TRUE 
                             WHERE ""UserId"" = @userId AND ""IsRead"" = FALSE";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();
                return command.ExecuteNonQuery() >= 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error marking all as read: " + ex.Message);
                return false;
            }
        }

        public bool ClearUserNotifications(int userId)
        {
            string query = @"DELETE FROM ""Notifications"" WHERE ""UserId"" = @userId";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", userId);
                connection.Open();
                return command.ExecuteNonQuery() >= 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error clearing notifications: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // ===== NOTICE / NEWS / ANNOUNCEMENT METHODS =====
        // ============================================================

        public bool CreateNotice(Notice notice, out string? errorMessage)
        {
            errorMessage = null;

            if (string.Equals(notice.Category, "News", StringComparison.OrdinalIgnoreCase))
            {
                notice.IsAnnouncement = false;
            }

            if (notice.PublishedDate == default(DateTime) ||
                notice.PublishedDate.Year < 2000 ||
                notice.PublishedDate.Year > 2100)
            {
                notice.PublishedDate = DateTime.Now;
            }

            string query = @"
                INSERT INTO ""Notices"" 
                (""Title"", ""Content"", ""Category"", ""ImageUrl"", 
                 ""PublishedDate"", ""IsAnnouncement"", ""IsActive"", ""CreatedAt"")
                VALUES 
                (@title, @content, @category, @imageUrl,
                 @publishedDate, @isAnnouncement, @isActive, @createdAt)
                RETURNING ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@title", notice.Title ?? "");
                command.Parameters.AddWithValue("@content", notice.Content ?? "");
                command.Parameters.AddWithValue("@category", string.IsNullOrWhiteSpace(notice.Category) ? "News" : notice.Category);
                command.Parameters.AddWithValue("@imageUrl", notice.ImageUrl ?? "");
                command.Parameters.AddWithValue("@publishedDate", notice.PublishedDate);
                command.Parameters.AddWithValue("@isAnnouncement", notice.IsAnnouncement);
                command.Parameters.AddWithValue("@isActive", notice.IsActive);
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    notice.Id = newId;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public List<Notice> GetAllNotices()
        {
            var notices = new List<Notice>();
            string query = @"SELECT * FROM ""Notices"" WHERE ""IsActive"" = TRUE ORDER BY ""PublishedDate"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    notices.Add(MapNotice(reader));
                }
                return notices;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting all notices: " + ex.Message);
                return new List<Notice>();
            }
        }

        public Notice? GetNoticeById(int id)
        {
            string query = @"SELECT * FROM ""Notices"" WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return MapNotice(reader);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting notice by id: " + ex.Message);
                return null;
            }
        }

        public bool UpdateNotice(Notice notice, out string? errorMessage)
        {
            errorMessage = null;

            if (string.Equals(notice.Category, "News", StringComparison.OrdinalIgnoreCase))
            {
                notice.IsAnnouncement = false;
            }

            if (notice.PublishedDate == default(DateTime) ||
                notice.PublishedDate.Year < 2000 ||
                notice.PublishedDate.Year > 2100)
            {
                notice.PublishedDate = DateTime.Now;
            }

            string query = @"
                UPDATE ""Notices"" SET 
                    ""Title"" = @title,
                    ""Content"" = @content,
                    ""Category"" = @category,
                    ""ImageUrl"" = @imageUrl,
                    ""PublishedDate"" = @publishedDate,
                    ""IsAnnouncement"" = @isAnnouncement,
                    ""IsActive"" = @isActive,
                    ""UpdatedAt"" = @updatedAt
                WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@id", notice.Id);
                command.Parameters.AddWithValue("@title", notice.Title ?? "");
                command.Parameters.AddWithValue("@content", notice.Content ?? "");
                command.Parameters.AddWithValue("@category", string.IsNullOrWhiteSpace(notice.Category) ? "News" : notice.Category);
                command.Parameters.AddWithValue("@imageUrl", notice.ImageUrl ?? "");
                command.Parameters.AddWithValue("@publishedDate", notice.PublishedDate);
                command.Parameters.AddWithValue("@isAnnouncement", notice.IsAnnouncement);
                command.Parameters.AddWithValue("@isActive", notice.IsActive);
                command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool DeleteNotice(int id)
        {
            string query = @"DELETE FROM ""Notices"" WHERE ""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting notice: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // ===== SITE SETTINGS METHODS (✅ UPDATED WITH BRANDING) =====
        // ============================================================

        public SiteSettings GetSiteSettings()
        {
            string query = @"SELECT * FROM ""SiteSettings"" ORDER BY ""Id"" LIMIT 1";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    var settings = new SiteSettings
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        HeroImageUrls = reader["HeroImageUrls"]?.ToString() ?? "",
                        SliderEnabled = reader["SliderEnabled"] as bool? ?? false,
                        SliderIntervalSeconds = Convert.ToInt32(reader["SliderIntervalSeconds"]),
                        PrimaryColor = reader["PrimaryColor"]?.ToString() ?? "#1F3B2C",
                        SecondaryColor = reader["SecondaryColor"]?.ToString() ?? "#F3F1E7",
                        AccentColor = reader["AccentColor"]?.ToString() ?? "#F4C744",
                        TextColor = reader["TextColor"]?.ToString() ?? "#4B5648",
                        NavbarBgColor = reader["NavbarBgColor"]?.ToString() ?? "#F3F1E7",
                        FooterBgColor = reader["FooterBgColor"]?.ToString() ?? "#F3F1E7",
                        CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                        UpdatedAt = reader["UpdatedAt"] as DateTime?
                    };

                    try
                    {
                        var tutorImgIdx = reader.GetOrdinal("TutorSectionImageUrl");
                        if (!reader.IsDBNull(tutorImgIdx))
                        {
                            settings.TutorSectionImageUrl = reader.GetString(tutorImgIdx);
                        }
                    }
                    catch
                    {
                        settings.TutorSectionImageUrl = "https://i.ibb.co.com/Hfpwnx46/111.png";
                    }

                    try
                    {
                        var idx = reader.GetOrdinal("SiteName");
                        if (!reader.IsDBNull(idx))
                            settings.SiteName = reader.GetString(idx);
                    }
                    catch { }

                    try
                    {
                        var idx = reader.GetOrdinal("NavbarLogoUrl");
                        if (!reader.IsDBNull(idx))
                            settings.NavbarLogoUrl = reader.GetString(idx);
                    }
                    catch { }

                    try
                    {
                        var idx = reader.GetOrdinal("FooterLogoUrl");
                        if (!reader.IsDBNull(idx))
                            settings.FooterLogoUrl = reader.GetString(idx);
                    }
                    catch { }

                    try
                    {
                        var idx = reader.GetOrdinal("LogoSizePx");
                        if (!reader.IsDBNull(idx))
                            settings.LogoSizePx = Convert.ToInt32(reader[idx]);
                    }
                    catch { }

                    try
                    {
                        var idx = reader.GetOrdinal("LogoShape");
                        if (!reader.IsDBNull(idx))
                            settings.LogoShape = reader.GetString(idx);
                    }
                    catch { }

                    return settings;
                }
                return new SiteSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting site settings: " + ex.Message);
                return new SiteSettings();
            }
        }

        public bool SaveSiteSettings(SiteSettings settings, out string? errorMessage)
        {
            errorMessage = null;

            string checkQuery = @"SELECT COUNT(*) FROM ""SiteSettings""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                long existing;
                using (var checkCmd = new NpgsqlCommand(checkQuery, connection))
                {
                    existing = Convert.ToInt64(checkCmd.ExecuteScalar());
                }

                bool HasColumn(string colName)
                {
                    try
                    {
                        using var colCheck = new NpgsqlCommand(@"
                            SELECT COUNT(*) FROM information_schema.columns 
                            WHERE table_name = 'SiteSettings' 
                            AND column_name = @col", connection);
                        colCheck.Parameters.AddWithValue("@col", colName);
                        return Convert.ToInt64(colCheck.ExecuteScalar()) > 0;
                    }
                    catch { return false; }
                }

                bool hasTutorColumn = HasColumn("TutorSectionImageUrl");
                bool hasSiteName = HasColumn("SiteName");
                bool hasNavLogo = HasColumn("NavbarLogoUrl");
                bool hasFootLogo = HasColumn("FooterLogoUrl");
                bool hasLogoSize = HasColumn("LogoSizePx");
                bool hasLogoShape = HasColumn("LogoShape");

                var setClauses = new List<string>
                {
                    @"""HeroImageUrls"" = @heroImageUrls",
                    @"""SliderEnabled"" = @sliderEnabled",
                    @"""SliderIntervalSeconds"" = @sliderInterval",
                    @"""PrimaryColor"" = @primaryColor",
                    @"""SecondaryColor"" = @secondaryColor",
                    @"""AccentColor"" = @accentColor",
                    @"""TextColor"" = @textColor",
                    @"""NavbarBgColor"" = @navbarBgColor",
                    @"""FooterBgColor"" = @footerBgColor"
                };

                var insertCols = new List<string>
                {
                    @"""HeroImageUrls""", @"""SliderEnabled""", @"""SliderIntervalSeconds""",
                    @"""PrimaryColor""", @"""SecondaryColor""", @"""AccentColor""",
                    @"""TextColor""", @"""NavbarBgColor""", @"""FooterBgColor"""
                };
                var insertVals = new List<string>
                {
                    "@heroImageUrls", "@sliderEnabled", "@sliderInterval",
                    "@primaryColor", "@secondaryColor", "@accentColor",
                    "@textColor", "@navbarBgColor", "@footerBgColor"
                };

                if (hasTutorColumn)
                {
                    setClauses.Add(@"""TutorSectionImageUrl"" = @tutorSectionImageUrl");
                    insertCols.Add(@"""TutorSectionImageUrl""");
                    insertVals.Add("@tutorSectionImageUrl");
                }

                if (hasSiteName)
                {
                    setClauses.Add(@"""SiteName"" = @siteName");
                    insertCols.Add(@"""SiteName""");
                    insertVals.Add("@siteName");
                }

                if (hasNavLogo)
                {
                    setClauses.Add(@"""NavbarLogoUrl"" = @navbarLogoUrl");
                    insertCols.Add(@"""NavbarLogoUrl""");
                    insertVals.Add("@navbarLogoUrl");
                }

                if (hasFootLogo)
                {
                    setClauses.Add(@"""FooterLogoUrl"" = @footerLogoUrl");
                    insertCols.Add(@"""FooterLogoUrl""");
                    insertVals.Add("@footerLogoUrl");
                }

                if (hasLogoSize)
                {
                    setClauses.Add(@"""LogoSizePx"" = @logoSizePx");
                    insertCols.Add(@"""LogoSizePx""");
                    insertVals.Add("@logoSizePx");
                }

                if (hasLogoShape)
                {
                    setClauses.Add(@"""LogoShape"" = @logoShape");
                    insertCols.Add(@"""LogoShape""");
                    insertVals.Add("@logoShape");
                }

                string query;
                if (existing > 0)
                {
                    query = $@"UPDATE ""SiteSettings"" SET {string.Join(", ", setClauses)}, ""UpdatedAt"" = @updatedAt WHERE ""Id"" = @id";
                }
                else
                {
                    query = $@"INSERT INTO ""SiteSettings"" ({string.Join(", ", insertCols)}, ""CreatedAt"")
                               VALUES ({string.Join(", ", insertVals)}, @createdAt)";
                }

                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@heroImageUrls", settings.HeroImageUrls ?? "");
                command.Parameters.AddWithValue("@sliderEnabled", settings.SliderEnabled);
                command.Parameters.AddWithValue("@sliderInterval", settings.SliderIntervalSeconds);
                command.Parameters.AddWithValue("@primaryColor", settings.PrimaryColor ?? "#1F3B2C");
                command.Parameters.AddWithValue("@secondaryColor", settings.SecondaryColor ?? "#F3F1E7");
                command.Parameters.AddWithValue("@accentColor", settings.AccentColor ?? "#F4C744");
                command.Parameters.AddWithValue("@textColor", settings.TextColor ?? "#4B5648");
                command.Parameters.AddWithValue("@navbarBgColor", settings.NavbarBgColor ?? "#F3F1E7");
                command.Parameters.AddWithValue("@footerBgColor", settings.FooterBgColor ?? "#F3F1E7");

                if (hasTutorColumn)
                {
                    command.Parameters.AddWithValue("@tutorSectionImageUrl",
                        string.IsNullOrWhiteSpace(settings.TutorSectionImageUrl)
                            ? "https://i.ibb.co.com/Hfpwnx46/111.png"
                            : settings.TutorSectionImageUrl);
                }

                if (hasSiteName)
                {
                    command.Parameters.AddWithValue("@siteName",
                        string.IsNullOrWhiteSpace(settings.SiteName)
                            ? "SMART TUTOR HUB"
                            : settings.SiteName.Trim());
                }

                if (hasNavLogo)
                {
                    command.Parameters.AddWithValue("@navbarLogoUrl",
                        string.IsNullOrWhiteSpace(settings.NavbarLogoUrl)
                            ? "https://i.ibb.co/fd9sY88Y/IMG-6916-1.png"
                            : settings.NavbarLogoUrl.Trim());
                }

                if (hasFootLogo)
                {
                    command.Parameters.AddWithValue("@footerLogoUrl",
                        string.IsNullOrWhiteSpace(settings.FooterLogoUrl)
                            ? "https://i.ibb.co/fd9sY88Y/IMG-6916-1.png"
                            : settings.FooterLogoUrl.Trim());
                }

                if (hasLogoSize)
                {
                    int safeSize = settings.LogoSizePx;
                    if (safeSize < 24) safeSize = 24;
                    if (safeSize > 100) safeSize = 100;
                    command.Parameters.AddWithValue("@logoSizePx", safeSize);
                }

                if (hasLogoShape)
                {
                    var shape = (settings.LogoShape ?? "circle").Trim().ToLowerInvariant();
                    if (shape != "circle" && shape != "square" && shape != "rounded")
                        shape = "circle";
                    command.Parameters.AddWithValue("@logoShape", shape);
                }

                if (existing > 0)
                {
                    command.Parameters.AddWithValue("@id", settings.Id);
                    command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);
                }
                else
                {
                    command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
                }

                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        // ============================================================
        // ===== ✅ TUITION POSTS FOR HOME PAGE =====
        // ============================================================
        public List<TuitionPost> GetHomePageTuitionPosts(int limit = 3)
        {
            var posts = new List<TuitionPost>();
            string query = @"
                SELECT * FROM ""TuitionPosts"" 
                WHERE ""Status"" = 'Open' AND ""ShowOnHome"" = TRUE 
                ORDER BY ""PostedDate"" DESC 
                LIMIT @limit";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@limit", limit);
                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    posts.Add(new TuitionPost
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Title = reader["Title"]?.ToString() ?? "",
                        StudentGender = reader["StudentGender"]?.ToString(),
                        StudentClass = reader["StudentClass"]?.ToString(),
                        Subjects = reader["Subjects"]?.ToString(),
                        DaysPerWeek = reader["DaysPerWeek"]?.ToString(),
                        PreferredTime = reader["PreferredTime"]?.ToString(),
                        Salary = reader["Salary"]?.ToString(),
                        Area = reader["Area"]?.ToString(),
                        MediaFee = reader["MediaFee"]?.ToString(),
                        TuitionCode = reader["TuitionCode"]?.ToString(),
                        SpecialNote = reader["SpecialNote"]?.ToString(),
                        TutorGenderPreference = reader["TutorGenderPreference"]?.ToString(),
                        Status = reader["Status"]?.ToString() ?? "Open",
                        ShowOnHome = reader["ShowOnHome"] as bool? ?? false,
                        PostedDate = reader["PostedDate"] as DateTime? ?? DateTime.Now
                    });
                }
                return posts;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetHomePageTuitionPosts: " + ex.Message);
                return posts;
            }
        }

        // ============================================================
        // ===== ✅ TOGGLE SHOW ON HOME =====
        // ============================================================
        public bool ToggleShowOnHome(int postId, bool showOnHome)
        {
            string query = @"UPDATE ""TuitionPosts"" SET ""ShowOnHome"" = @showOnHome, ""UpdatedAt"" = @updatedAt WHERE ""Id"" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@showOnHome", showOnHome);
                command.Parameters.AddWithValue("@updatedAt", DateTime.UtcNow);
                command.Parameters.AddWithValue("@id", postId);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error ToggleShowOnHome: " + ex.Message);
                return false;
            }
        }

        // ============================================================
        // ===== TEACH WANTED POSTS (FindTutor Page) =====
        // ============================================================

        public List<TeachWantedPost> GetAllTeachWantedPosts(string? currentUserId = null)
        {
            var list = new List<TeachWantedPost>();
            string query = @"
                SELECT p.""Id"", p.""UserId"", p.""Content"", p.""ImageUrl"", p.""LikeCount"", p.""CreatedAt"",
                       u.""FullName"", u.""UserName"", u.""ProfileImageLink""
                FROM ""TeachWantedPosts"" p
                LEFT JOIN ""Users"" u ON p.""UserId"" = u.""Id""
                ORDER BY p.""CreatedAt"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var post = new TeachWantedPost
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        Content = reader["Content"]?.ToString() ?? "",
                        ImageUrl = reader["ImageUrl"]?.ToString(),
                        LikeCount = Convert.ToInt32(reader["LikeCount"]),
                        CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                        UserFullName = reader["FullName"]?.ToString() ?? "Teacher",
                        UserName = reader["UserName"]?.ToString() ?? "",
                        UserProfileImage = reader["ProfileImageLink"]?.ToString()
                    };
                    list.Add(post);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetAllTeachWantedPosts: " + ex.Message);
                return list;
            }

            foreach (var post in list)
            {
                post.Comments = GetTeachWantedComments(post.Id, currentUserId);
            }

            return list;
        }

        public List<TeachWantedComment> GetTeachWantedComments(int postId, string? currentUserId = null)
        {
            var allComments = new List<TeachWantedComment>();
            int uid = int.TryParse(currentUserId, out var parsedUid) ? parsedUid : -1;

            string query = @"
                SELECT c.""Id"", c.""PostId"", c.""UserId"", c.""Content"", c.""ParentId"", c.""CreatedAt"",
                       u.""FullName"", u.""UserName"", u.""ProfileImageLink"",
                       (SELECT COUNT(*) FROM ""TeachWantedCommentLikes"" cl WHERE cl.""CommentId"" = c.""Id"") AS ""LikeCount"",
                       EXISTS (SELECT 1 FROM ""TeachWantedCommentLikes"" cl2
                               WHERE cl2.""CommentId"" = c.""Id"" AND cl2.""UserId"" = @uid) AS ""IsLiked""
                FROM ""TeachWantedComments"" c
                LEFT JOIN ""Users"" u ON c.""UserId"" = u.""Id""
                WHERE c.""PostId"" = @postId
                ORDER BY c.""CreatedAt"" ASC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@postId", postId);
                command.Parameters.AddWithValue("@uid", uid);
                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    allComments.Add(new TeachWantedComment
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        PostId = reader.GetInt32(reader.GetOrdinal("PostId")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        Content = reader["Content"]?.ToString() ?? "",
                        ParentId = reader["ParentId"] != DBNull.Value ? Convert.ToInt32(reader["ParentId"]) : (int?)null,
                        CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                        UserFullName = reader["FullName"]?.ToString() ?? "User",
                        UserName = reader["UserName"]?.ToString() ?? "",
                        UserProfileImage = reader["ProfileImageLink"]?.ToString(),
                        LikeCount = Convert.ToInt32(reader["LikeCount"]),
                        IsLikedByMe = Convert.ToBoolean(reader["IsLiked"])
                    });
                }

                var byId = allComments.ToDictionary(c => c.Id);
                var topLevel = allComments
                    .Where(c => c.ParentId == null || !byId.ContainsKey(c.ParentId.Value))
                    .ToList();
                var topSet = new HashSet<int>(topLevel.Select(c => c.Id));

                foreach (var c in allComments)
                {
                    if (topSet.Contains(c.Id)) continue;

                    var root = c;
                    int guard = 0;
                    while (root.ParentId.HasValue && byId.TryGetValue(root.ParentId.Value, out var p) && guard++ < 50)
                        root = p;

                    root.Replies.Add(c);
                }

                return topLevel;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetTeachWantedComments: " + ex.Message);
                return new List<TeachWantedComment>();
            }
        }

        public (bool isLiked, int count) ToggleTeachCommentLike(int commentId, int userId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                bool exists;
                using (var checkCmd = new NpgsqlCommand(
                    @"SELECT COUNT(*) FROM ""TeachWantedCommentLikes"" WHERE ""CommentId"" = @cid AND ""UserId"" = @uid", connection))
                {
                    checkCmd.Parameters.AddWithValue("@cid", commentId);
                    checkCmd.Parameters.AddWithValue("@uid", userId);
                    exists = Convert.ToInt64(checkCmd.ExecuteScalar()) > 0;
                }

                if (exists)
                {
                    using var delCmd = new NpgsqlCommand(
                        @"DELETE FROM ""TeachWantedCommentLikes"" WHERE ""CommentId"" = @cid AND ""UserId"" = @uid", connection);
                    delCmd.Parameters.AddWithValue("@cid", commentId);
                    delCmd.Parameters.AddWithValue("@uid", userId);
                    delCmd.ExecuteNonQuery();
                }
                else
                {
                    using var insCmd = new NpgsqlCommand(
                        @"INSERT INTO ""TeachWantedCommentLikes"" (""CommentId"", ""UserId"", ""CreatedAt"")
                          VALUES (@cid, @uid, @createdAt)
                          ON CONFLICT DO NOTHING", connection);
                    insCmd.Parameters.AddWithValue("@cid", commentId);
                    insCmd.Parameters.AddWithValue("@uid", userId);
                    insCmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
                    insCmd.ExecuteNonQuery();
                }

                int count;
                using (var cntCmd = new NpgsqlCommand(
                    @"SELECT COUNT(*) FROM ""TeachWantedCommentLikes"" WHERE ""CommentId"" = @cid", connection))
                {
                    cntCmd.Parameters.AddWithValue("@cid", commentId);
                    count = Convert.ToInt32(cntCmd.ExecuteScalar());
                }

                return (!exists, count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error ToggleTeachCommentLike: " + ex.Message);
                return (false, 0);
            }
        }

        public bool CreateTeachWantedPost(TeachWantedPost post, out string? errorMessage)
        {
            errorMessage = null;
            string query = @"
                INSERT INTO ""TeachWantedPosts"" (""UserId"", ""Content"", ""ImageUrl"", ""LikeCount"", ""CreatedAt"")
                VALUES (@userId, @content, @imageUrl, 0, @createdAt)
                RETURNING ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@userId", post.UserId);
                command.Parameters.AddWithValue("@content", post.Content ?? "");
                command.Parameters.AddWithValue("@imageUrl", (object?)post.ImageUrl ?? DBNull.Value);
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    post.Id = newId;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool UpdateTeachWantedPost(int postId, int requesterId, string content,
            bool imageChanged, string? newImageUrl,
            out string? oldImageUrl, out string? finalImageUrl, out string? errorMessage)
        {
            oldImageUrl = null;
            finalImageUrl = null;
            errorMessage = null;

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                int ownerId;
                using (var getCmd = new NpgsqlCommand(
                    @"SELECT ""UserId"", ""ImageUrl"" FROM ""TeachWantedPosts"" WHERE ""Id"" = @id", connection))
                {
                    getCmd.Parameters.AddWithValue("@id", postId);
                    using var r = getCmd.ExecuteReader();
                    if (!r.Read()) { errorMessage = "Post not found."; return false; }
                    ownerId = r.GetInt32(0);
                    oldImageUrl = r.IsDBNull(1) ? null : r.GetString(1);
                }

                if (ownerId != requesterId)
                {
                    errorMessage = "You can only edit your own post.";
                    return false;
                }

                finalImageUrl = imageChanged ? newImageUrl : oldImageUrl;

                if (string.IsNullOrWhiteSpace(content) && string.IsNullOrEmpty(finalImageUrl))
                {
                    errorMessage = "Post cannot be empty.";
                    return false;
                }

                using var updCmd = new NpgsqlCommand(
                    @"UPDATE ""TeachWantedPosts"" SET ""Content"" = @content, ""ImageUrl"" = @img WHERE ""Id"" = @id", connection);
                updCmd.Parameters.AddWithValue("@content", content ?? "");
                updCmd.Parameters.AddWithValue("@img", (object?)finalImageUrl ?? DBNull.Value);
                updCmd.Parameters.AddWithValue("@id", postId);
                return updCmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool DeleteTeachWantedPost(int postId, int requesterId, bool isAdmin,
            out string? imageUrl, out string? errorMessage)
        {
            imageUrl = null;
            errorMessage = null;

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                using var tx = connection.BeginTransaction();

                int ownerId;
                using (var getCmd = new NpgsqlCommand(
                    @"SELECT ""UserId"", ""ImageUrl"" FROM ""TeachWantedPosts"" WHERE ""Id"" = @id", connection, tx))
                {
                    getCmd.Parameters.AddWithValue("@id", postId);
                    using var r = getCmd.ExecuteReader();
                    if (!r.Read()) { errorMessage = "Post not found."; return false; }
                    ownerId = r.GetInt32(0);
                    imageUrl = r.IsDBNull(1) ? null : r.GetString(1);
                }

                if (ownerId != requesterId && !isAdmin)
                {
                    errorMessage = "You can only delete your own post.";
                    return false;
                }

                string[] statements =
                {
                    @"DELETE FROM ""TeachWantedCommentLikes"" WHERE ""CommentId"" IN (SELECT ""Id"" FROM ""TeachWantedComments"" WHERE ""PostId"" = @id)",
                    @"DELETE FROM ""TeachWantedComments"" WHERE ""PostId"" = @id",
                    @"DELETE FROM ""TeachWantedLikes"" WHERE ""PostId"" = @id",
                    @"DELETE FROM ""TeachWantedPosts"" WHERE ""Id"" = @id"
                };

                foreach (var sql in statements)
                {
                    using var cmd = new NpgsqlCommand(sql, connection, tx);
                    cmd.Parameters.AddWithValue("@id", postId);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool AddTeachWantedComment(TeachWantedComment comment, out string? errorMessage)
        {
            errorMessage = null;
            string query = @"
                INSERT INTO ""TeachWantedComments"" (""PostId"", ""UserId"", ""Content"", ""ParentId"", ""CreatedAt"")
                VALUES (@postId, @userId, @content, @parentId, @createdAt)
                RETURNING ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@postId", comment.PostId);
                command.Parameters.AddWithValue("@userId", comment.UserId);
                command.Parameters.AddWithValue("@content", comment.Content ?? "");
                command.Parameters.AddWithValue("@parentId", (object?)comment.ParentId ?? DBNull.Value);
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    comment.Id = newId;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public (bool isLiked, int newCount) ToggleTeachWantedLike(int postId, int userId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                string checkQuery = @"SELECT COUNT(*) FROM ""TeachWantedLikes"" WHERE ""PostId"" = @postId AND ""UserId"" = @userId";
                using var checkCmd = new NpgsqlCommand(checkQuery, connection);
                checkCmd.Parameters.AddWithValue("@postId", postId);
                checkCmd.Parameters.AddWithValue("@userId", userId);
                long exists = Convert.ToInt64(checkCmd.ExecuteScalar());

                bool isLiked;
                int change = 1;

                if (exists > 0)
                {
                    string delQuery = @"DELETE FROM ""TeachWantedLikes"" WHERE ""PostId"" = @postId AND ""UserId"" = @userId";
                    using var delCmd = new NpgsqlCommand(delQuery, connection);
                    delCmd.Parameters.AddWithValue("@postId", postId);
                    delCmd.Parameters.AddWithValue("@userId", userId);
                    delCmd.ExecuteNonQuery();
                    isLiked = false;
                    change = -1;
                }
                else
                {
                    string insQuery = @"INSERT INTO ""TeachWantedLikes"" (""PostId"", ""UserId"", ""CreatedAt"") VALUES (@postId, @userId, @createdAt)";
                    using var insCmd = new NpgsqlCommand(insQuery, connection);
                    insCmd.Parameters.AddWithValue("@postId", postId);
                    insCmd.Parameters.AddWithValue("@userId", userId);
                    insCmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);
                    insCmd.ExecuteNonQuery();
                    isLiked = true;
                }

                string updQuery = @"UPDATE ""TeachWantedPosts"" SET ""LikeCount"" = ""LikeCount"" + @change WHERE ""Id"" = @postId RETURNING ""LikeCount""";
                using var updCmd = new NpgsqlCommand(updQuery, connection);
                updCmd.Parameters.AddWithValue("@change", change);
                updCmd.Parameters.AddWithValue("@postId", postId);
                int newCount = Convert.ToInt32(updCmd.ExecuteScalar());

                return (isLiked, newCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error ToggleTeachWantedLike: " + ex.Message);
                return (false, 0);
            }
        }

        // ============================================================
        // ===== PRIVATE MAPPERS =====
        // ============================================================

        private User MapUser(NpgsqlDataReader reader)
        {
            return new User
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserName = reader["UserName"]?.ToString() ?? string.Empty,
                Password = reader["Password"]?.ToString() ?? string.Empty,
                FullName = reader["FullName"]?.ToString() ?? string.Empty,
                Email = reader["Email"]?.ToString() ?? string.Empty,
                CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                LastLoginAt = reader["LastLoginAt"] as DateTime?,
                Role = reader["Role"]?.ToString() ?? "User",
                IsActive = reader["IsActive"] as bool? ?? true,
                MobileNumber = reader["MobileNumber"]?.ToString(),
                ProfileImage = reader["ProfileImage"]?.ToString(),
                Gender = reader["Gender"]?.ToString(),
                AgeRange = reader["AgeRange"]?.ToString(),
                PrimaryDeviceType = reader["PrimaryDeviceType"]?.ToString(),
                YearsOfExperience = reader["YearsOfExperience"]?.ToString(),
                AreaType = reader["AreaType"]?.ToString(),
                Country = reader["Country"]?.ToString(),
                StreetAddress = reader["StreetAddress"]?.ToString(),
                PermanentAddress = reader["PermanentAddress"]?.ToString(),
                EducationLevel = reader["EducationLevel"]?.ToString(),
                CurrentStudyStatus = reader["CurrentStudyStatus"]?.ToString(),
                ExamDegreeTitle = reader["ExamDegreeTitle"]?.ToString(),
                InstitutionName = reader["InstitutionName"]?.ToString(),
                PassingYear = reader["PassingYear"]?.ToString(),
                IsCSEStudent = reader["IsCSEStudent"] as bool?,
                CvLink = reader["CvLink"]?.ToString(),
                GithubProfile = reader["GithubProfile"]?.ToString(),
                PortfolioLink = reader["PortfolioLink"]?.ToString(),
                LinkedInProfile = reader["LinkedInProfile"]?.ToString(),
                ProfileImageLink = reader["ProfileImageLink"]?.ToString()
            };
        }

        private TeacherRequest MapTeacherRequest(NpgsqlDataReader reader)
        {
            return new TeacherRequest
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                FullName = reader["FullName"]?.ToString() ?? string.Empty,
                Email = reader["Email"]?.ToString() ?? string.Empty,
                MobileNumber = reader["MobileNumber"]?.ToString() ?? string.Empty,
                Education = reader["Education"]?.ToString() ?? string.Empty,
                Institution = reader["Institution"]?.ToString() ?? string.Empty,
                SubjectExpertise = reader["SubjectExpertise"]?.ToString() ?? string.Empty,
                Experience = reader["Experience"]?.ToString() ?? string.Empty,
                TeachingStyle = reader["TeachingStyle"]?.ToString() ?? string.Empty,
                AvailableDays = reader["AvailableDays"]?.ToString() ?? string.Empty,
                PreferredTime = reader["PreferredTime"]?.ToString() ?? string.Empty,
                HourlyRate = reader["HourlyRate"]?.ToString() ?? string.Empty,
                CvLink = reader["CvLink"]?.ToString() ?? string.Empty,
                WhyTeach = reader["WhyTeach"]?.ToString() ?? string.Empty,
                Status = reader["Status"]?.ToString() ?? "Pending",
                RequestDate = reader["RequestDate"] as DateTime? ?? DateTime.UtcNow,
                ResponseDate = reader["ResponseDate"] as DateTime?,
                AdminNote = reader["AdminNote"]?.ToString()
            };
        }

        private TutorRequest MapTutorRequest(NpgsqlDataReader reader)
        {
            return new TutorRequest
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                TutorId = reader["TutorId"]?.ToString(),
                TutorName = reader["TutorName"]?.ToString(),
                StudentName = reader["StudentName"]?.ToString() ?? "",
                PhoneNumber = reader["PhoneNumber"]?.ToString(),
                WhatsAppNumber = reader["WhatsAppNumber"]?.ToString(),
                Email = reader["Email"]?.ToString(),
                StudentClass = reader["StudentClass"]?.ToString(),
                RequiredSubject = reader["RequiredSubject"]?.ToString(),
                PreferredArea = reader["PreferredArea"]?.ToString(),
                TeachingMode = reader["TeachingMode"]?.ToString(),
                PreferredDays = reader["PreferredDays"]?.ToString(),
                PreferredTime = reader["PreferredTime"]?.ToString(),
                MonthlyBudget = reader["MonthlyBudget"]?.ToString(),
                AdditionalNotes = reader["AdditionalNotes"]?.ToString(),
                RequestType = reader["RequestType"]?.ToString() ?? "SpecificTutor",
                RequestId = reader["RequestId"]?.ToString(),
                Status = reader["Status"]?.ToString() ?? "Pending",
                AdminNote = reader["AdminNote"]?.ToString(),
                CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                UpdatedAt = reader["UpdatedAt"] as DateTime?
            };
        }

        private Notice MapNotice(NpgsqlDataReader reader)
        {
            return new Notice
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader["Title"]?.ToString() ?? "",
                Content = reader["Content"]?.ToString() ?? "",
                Category = reader["Category"]?.ToString() ?? "News",
                ImageUrl = reader["ImageUrl"]?.ToString() ?? "",
                PublishedDate = reader["PublishedDate"] as DateTime? ?? DateTime.UtcNow,
                IsAnnouncement = reader["IsAnnouncement"] as bool? ?? false,
                IsActive = reader["IsActive"] as bool? ?? true,
                CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                UpdatedAt = reader["UpdatedAt"] as DateTime?
            };
        }

        // ============================================================
        // ===== HELPDESK — POSTS =====
        // ============================================================

        public bool CreateHelpdeskPost(HelpdeskPost post, out string? errorMessage)
        {
            errorMessage = null;
            string query = @"
                INSERT INTO ""HelpdeskPosts"" 
                (""UserId"", ""Title"", ""Content"", ""MediaUrl"", ""MediaType"", 
                 ""IsAdminPost"", ""IsAcknowledged"", ""Status"", ""CreatedAt"")
                VALUES 
                (@userId, @title, @content, @mediaUrl, @mediaType,
                 @isAdminPost, FALSE, 'Open', @createdAt)
                RETURNING ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@userId", post.UserId);
                command.Parameters.AddWithValue("@title", post.Title ?? "");
                command.Parameters.AddWithValue("@content", post.Content ?? "");
                command.Parameters.AddWithValue("@mediaUrl", (object?)post.MediaUrl ?? DBNull.Value);
                command.Parameters.AddWithValue("@mediaType", (object?)post.MediaType ?? DBNull.Value);
                command.Parameters.AddWithValue("@isAdminPost", post.IsAdminPost);
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    post.Id = newId;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public List<HelpdeskPost> GetHelpdeskPosts(string filter = "all", int? userId = null, string? search = null)
        {
            var list = new List<HelpdeskPost>();

            string query = @"
                SELECT p.""Id"", p.""UserId"", p.""Title"", p.""Content"", p.""MediaUrl"", p.""MediaType"",
                       p.""IsAdminPost"", p.""IsAcknowledged"", p.""Status"", p.""CreatedAt"", p.""UpdatedAt"",
                       u.""FullName"", u.""UserName"", u.""Role"", u.""ProfileImageLink"",
                       (SELECT COUNT(*) FROM ""HelpdeskComments"" WHERE ""PostId"" = p.""Id"") AS CommentCount
                FROM ""HelpdeskPosts"" p
                LEFT JOIN ""Users"" u ON p.""UserId"" = u.""Id""
                WHERE 1=1";

            if (filter == "mine" && userId.HasValue)
                query += @" AND p.""UserId"" = @userId";

            if (filter == "admin")
                query += @" AND p.""IsAdminPost"" = TRUE";

            if (!string.IsNullOrWhiteSpace(search))
                query += @" AND (p.""Title"" ILIKE @search OR p.""Content"" ILIKE @search)";

            query += @" ORDER BY p.""CreatedAt"" DESC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                if (filter == "mine" && userId.HasValue)
                    command.Parameters.AddWithValue("@userId", userId.Value);

                if (!string.IsNullOrWhiteSpace(search))
                    command.Parameters.AddWithValue("@search", $"%{search.Trim()}%");

                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new HelpdeskPost
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        Title = reader["Title"]?.ToString() ?? "",
                        Content = reader["Content"]?.ToString() ?? "",
                        MediaUrl = reader["MediaUrl"]?.ToString(),
                        MediaType = reader["MediaType"]?.ToString(),
                        IsAdminPost = reader["IsAdminPost"] as bool? ?? false,
                        IsAcknowledged = reader["IsAcknowledged"] as bool? ?? false,
                        Status = reader["Status"]?.ToString() ?? "Open",
                        CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                        UpdatedAt = reader["UpdatedAt"] as DateTime?,
                        UserFullName = reader["FullName"]?.ToString() ?? "Unknown",
                        UserName = reader["UserName"]?.ToString() ?? "",
                        UserRole = reader["Role"]?.ToString() ?? "User",
                        UserProfileImage = reader["ProfileImageLink"]?.ToString(),
                        CommentCount = Convert.ToInt32(reader["CommentCount"])
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetHelpdeskPosts: " + ex.Message);
                return list;
            }
        }

        public HelpdeskPost? GetHelpdeskPostById(int id)
        {
            string query = @"
                SELECT p.""Id"", p.""UserId"", p.""Title"", p.""Content"", p.""MediaUrl"", p.""MediaType"",
                       p.""IsAdminPost"", p.""IsAcknowledged"", p.""Status"", p.""CreatedAt"", p.""UpdatedAt"",
                       u.""FullName"", u.""UserName"", u.""Role"", u.""ProfileImageLink"",
                       (SELECT COUNT(*) FROM ""HelpdeskComments"" WHERE ""PostId"" = p.""Id"") AS CommentCount
                FROM ""HelpdeskPosts"" p
                LEFT JOIN ""Users"" u ON p.""UserId"" = u.""Id""
                WHERE p.""Id"" = @id";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new HelpdeskPost
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        Title = reader["Title"]?.ToString() ?? "",
                        Content = reader["Content"]?.ToString() ?? "",
                        MediaUrl = reader["MediaUrl"]?.ToString(),
                        MediaType = reader["MediaType"]?.ToString(),
                        IsAdminPost = reader["IsAdminPost"] as bool? ?? false,
                        IsAcknowledged = reader["IsAcknowledged"] as bool? ?? false,
                        Status = reader["Status"]?.ToString() ?? "Open",
                        CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                        UpdatedAt = reader["UpdatedAt"] as DateTime?,
                        UserFullName = reader["FullName"]?.ToString() ?? "Unknown",
                        UserName = reader["UserName"]?.ToString() ?? "",
                        UserRole = reader["Role"]?.ToString() ?? "User",
                        UserProfileImage = reader["ProfileImageLink"]?.ToString(),
                        CommentCount = Convert.ToInt32(reader["CommentCount"])
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetHelpdeskPostById: " + ex.Message);
                return null;
            }
        }

        public bool DeleteHelpdeskPost(int id, int requestingUserId, bool isAdmin)
        {
            string query = isAdmin
                ? @"DELETE FROM ""HelpdeskPosts"" WHERE ""Id"" = @id"
                : @"DELETE FROM ""HelpdeskPosts"" WHERE ""Id"" = @id AND ""UserId"" = @userId";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                if (!isAdmin)
                    command.Parameters.AddWithValue("@userId", requestingUserId);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error DeleteHelpdeskPost: " + ex.Message);
                return false;
            }
        }

        public bool AcknowledgeHelpdeskPost(int id, bool acknowledged = true)
        {
            string query = @"UPDATE ""HelpdeskPosts"" SET ""IsAcknowledged"" = @ack, ""UpdatedAt"" = @upd WHERE ""Id"" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@ack", acknowledged);
                command.Parameters.AddWithValue("@upd", DateTime.UtcNow);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch { return false; }
        }

        public bool UpdateHelpdeskPostStatus(int id, string status)
        {
            string query = @"UPDATE ""HelpdeskPosts"" SET ""Status"" = @status, ""UpdatedAt"" = @upd WHERE ""Id"" = @id";
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@status", status ?? "Open");
                command.Parameters.AddWithValue("@upd", DateTime.UtcNow);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch { return false; }
        }

        // ============================================================
        // ===== HELPDESK — COMMENTS =====
        // ============================================================

        public bool CreateHelpdeskComment(HelpdeskComment comment, out string? errorMessage)
        {
            errorMessage = null;
            string query = @"
                INSERT INTO ""HelpdeskComments"" 
                (""PostId"", ""UserId"", ""Content"", ""MediaUrl"", ""MediaType"", 
                 ""IsAdminReply"", ""ParentCommentId"", ""CreatedAt"")
                VALUES 
                (@postId, @userId, @content, @mediaUrl, @mediaType,
                 @isAdminReply, @parentId, @createdAt)
                RETURNING ""Id""";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);

                command.Parameters.AddWithValue("@postId", comment.PostId);
                command.Parameters.AddWithValue("@userId", comment.UserId);
                command.Parameters.AddWithValue("@content", comment.Content ?? "");
                command.Parameters.AddWithValue("@mediaUrl", (object?)comment.MediaUrl ?? DBNull.Value);
                command.Parameters.AddWithValue("@mediaType", (object?)comment.MediaType ?? DBNull.Value);
                command.Parameters.AddWithValue("@isAdminReply", comment.IsAdminReply);
                command.Parameters.AddWithValue("@parentId", (object?)comment.ParentCommentId ?? DBNull.Value);
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

                connection.Open();
                var result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    comment.Id = newId;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public List<HelpdeskComment> GetHelpdeskComments(int postId)
        {
            var allComments = new List<HelpdeskComment>();

            string query = @"
                SELECT c.""Id"", c.""PostId"", c.""UserId"", c.""Content"", c.""MediaUrl"", c.""MediaType"",
                       c.""IsAdminReply"", c.""ParentCommentId"", c.""CreatedAt"",
                       u.""FullName"", u.""UserName"", u.""Role"", u.""ProfileImageLink""
                FROM ""HelpdeskComments"" c
                LEFT JOIN ""Users"" u ON c.""UserId"" = u.""Id""
                WHERE c.""PostId"" = @postId
                ORDER BY c.""CreatedAt"" ASC";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@postId", postId);
                connection.Open();
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    allComments.Add(new HelpdeskComment
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        PostId = reader.GetInt32(reader.GetOrdinal("PostId")),
                        UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                        Content = reader["Content"]?.ToString() ?? "",
                        MediaUrl = reader["MediaUrl"]?.ToString(),
                        MediaType = reader["MediaType"]?.ToString(),
                        IsAdminReply = reader["IsAdminReply"] as bool? ?? false,
                        ParentCommentId = reader["ParentCommentId"] != DBNull.Value
                            ? Convert.ToInt32(reader["ParentCommentId"]) : (int?)null,
                        CreatedAt = reader["CreatedAt"] as DateTime? ?? DateTime.UtcNow,
                        UserFullName = reader["FullName"]?.ToString() ?? "Unknown",
                        UserName = reader["UserName"]?.ToString() ?? "",
                        UserRole = reader["Role"]?.ToString() ?? "User",
                        UserProfileImage = reader["ProfileImageLink"]?.ToString()
                    });
                }

                var topLevel = allComments.Where(c => c.ParentCommentId == null).ToList();
                foreach (var parent in topLevel)
                {
                    parent.Replies = allComments.Where(c => c.ParentCommentId == parent.Id).ToList();
                    parent.ReplyCount = parent.Replies.Count;
                }

                return topLevel;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error GetHelpdeskComments: " + ex.Message);
                return allComments;
            }
        }

        public bool DeleteHelpdeskComment(int commentId, int requestingUserId, bool isAdmin)
        {
            string query = isAdmin
                ? @"DELETE FROM ""HelpdeskComments"" WHERE ""Id"" = @id"
                : @"DELETE FROM ""HelpdeskComments"" WHERE ""Id"" = @id AND ""UserId"" = @userId";

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", commentId);
                if (!isAdmin)
                    command.Parameters.AddWithValue("@userId", requestingUserId);

                connection.Open();
                return command.ExecuteNonQuery() > 0;
            }
            catch { return false; }
        }
    }
}