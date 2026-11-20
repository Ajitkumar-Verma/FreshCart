# Database scripts

`Tables`, `Stored Procedures`, `Security`, and `UserDefinedTypes` are compiled into the SSDT database project. `Script.PostDeployment.sql` runs the idempotent seed script after the schema is published. Use SSDT publish to deploy the database. Add repeatable data migrations here and include them from the post-deployment script in numeric order. Do not put credentials in checked-in scripts.
