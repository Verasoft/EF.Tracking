using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Linq;

namespace Verasoft.EF.Tracking.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private const string UT_ProjectName = "UnitTest-Project1";
        private const string UT_DepartmentName = "UnitTest-Department1";

        [TestInitialize]
        public void TestInitialize()
        {
            TrackingContext.Current.ClearSubscriptions();
        }

        [TestMethod]
        public void AfterSaveChanges_Entity_RelatedEntity_Added()
        {
            string result = string.Empty;
            string resultFormat = "AfterSaveChanges_Entity_RelatedEntity_Added_{0}_{1}";

            TrackingContext.Current.Subscribe<EFTrackingContext>(TrackingEvent.AfterSaveChanges, (s) =>
                {
                    s.For<Project>()
                        .Where(x => x.Departments, RelatedEntityTrackingTypes.Added)
                        .Perform((content, item) =>
                            {
                                result = string.Format(resultFormat, item.Entity.Name, item.RelatedEntity.Name);
                            });
                });

            using(var db = new EFTrackingContext())
            {
                var project = db.Projects.SingleOrDefault(x=>x.Name == UT_ProjectName);
                if (project == null)
                {
                    project = new Project { Name = UT_ProjectName };
                    db.Projects.Add(project);
                    db.SaveChanges();
                }

                var department = new Department{ Name = UT_DepartmentName};
                project.Departments.Add(department);
                db.SaveChanges();

                Assert.AreEqual(string.Format(resultFormat, project.Name, department.Name), result, "AfterSaveChanges_Entity_RelatedEntity_Added - Failed");
            }
        }

        [TestMethod]
        public void AfterSaveChanges_Entity_RelatedEntity_Deleted()
        {
            bool deleted = false;

            TrackingContext.Current.Subscribe<EFTrackingContext>(TrackingEvent.AfterSaveChanges, (s) =>
            {
                s.For<Project>()
                    .Where(x => x.Departments, RelatedEntityTrackingTypes.Deleted)
                    .Perform((content, item) =>
                    {
                        deleted = true;
                    });
            });

            using (var db = new EFTrackingContext())
            {
                var project = db.Projects.SingleOrDefault(x => x.Name == UT_ProjectName);
                if (project == null)
                {
                    project = new Project { Name = UT_ProjectName };
                    db.Projects.Add(project);
                    db.SaveChanges();
                }

                var department = project.Departments.SingleOrDefault(x => x.Name == UT_DepartmentName);
                if (department == null)
                {
                    department = new Department { Name = UT_DepartmentName };
                    project.Departments.Add(department);
                    db.SaveChanges();
                }

                project.Departments.Remove(department);
                db.SaveChanges();

                Assert.IsTrue(deleted, "AfterSaveChanges_Entity_RelatedEntity_Deleted - Failed");
            }
        }

        [TestMethod]
        public void AfterSaveChanges_Entity_Modified()
        {
            string result = "";

            TrackingContext.Current.Subscribe<EFTrackingContext>(TrackingEvent.AfterSaveChanges, (s) =>
            {
                s.For<Project>()
                    .Where(x=>x.Name)
                    .Perform((content, item) =>
                    {
                        result = string.Format("{0}--{1}", item.OriginalValue, item.CurrentValue);
                    });
            });

            using (var db = new EFTrackingContext())
            {
                var project = db.Projects.SingleOrDefault(x => x.Name == UT_ProjectName);
                if (project == null)
                {
                    project = new Project { Name = UT_ProjectName };
                    db.Projects.Add(project);
                    db.SaveChanges();
                }

                var token = DateTime.Now.ToString("yyyyMMddHHmmss");
                project.Name = UT_ProjectName + "_" + token;
                db.SaveChanges();

                Assert.AreEqual(string.Format("{0}--{1}", UT_ProjectName, project.Name), result, "AfterSaveChanges_Entity_Modified - Failed");
            }
        }

        [TestMethod]
        public void AfterSaveChanges_Entity_Ignore_Modified()
        {
            int result = 0;

            TrackingContext.Current.Subscribe<EFTrackingContext>(TrackingEvent.AfterSaveChanges, (s) =>
            {
                s.For<Project>()
                    .Where(EntityTrackingTypes.Modified)
                    .Perform((content, item) =>
                    {
                        result++;
                    });
            });

            using (var db = new EFTrackingContext())
            {
                var project = db.Projects.SingleOrDefault(x => x.Name == UT_ProjectName);
                if (project == null)
                {
                    project = new Project { Name = UT_ProjectName };
                    db.Projects.Add(project);
                    db.SaveChanges();
                }

                var token1 = DateTime.Now.ToString("Test1_yyyyMMddHHmmss");
                project.Name = UT_ProjectName + "_" + token1;
                db.SaveChanges();

                Assert.AreEqual(1, result, "AfterSaveChanges_Entity_Ignore_Modified - Failed");

                var token2 = DateTime.Now.ToString("Test2_yyyyMMddHHmmss");
                project.Name = UT_ProjectName + "_" + token2;
                db.SaveChanges((x) =>
                    {
                        x.IgnoreEntity<EFTrackingContext, Project>(EntityTrackingTypes.Modified);
                    });

                Assert.AreEqual(1, result, "AfterSaveChanges_Entity_Ignore_Modified - Failed");
            }
        }
    }
}
