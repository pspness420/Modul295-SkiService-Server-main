const { Sequelize } = require('sequelize');

// Initialize Sequelize with SQL Server Authentication
const sequelize = new Sequelize('SkiServiceDB', 'ski_user', 'Test1234', {
  host: 'BZ_WIN_YANNICK_',
  dialect: 'mssql',
  dialectOptions: {
    options: {
      encrypt: true, // Falls Verschlüsselung erforderlich
      trustServerCertificate: true, // Serverzertifikat akzeptieren
    },
  },
  logging: false, // Deaktiviert Logging
});

// Test the database connection
sequelize
  .authenticate()
  .then(() => console.log('Connection to the database has been established successfully.'))
  .catch((err) => console.error('Unable to connect to the database:', err));

module.exports = sequelize;
