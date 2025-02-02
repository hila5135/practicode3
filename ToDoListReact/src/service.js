import axios from 'axios';

const apiUrl = "https://localhost:7083";
axios.defaults.baseURL = 'https://localhost:7083';  // כתובת ה-API שלך
axios.defaults.headers.common['Content-Type'] = 'application/json';


// הוספת interceptor לשגיאות
axios.interceptors.response.use(
  response => response, // אם לא הייתה שגיאה, מחזירים את התגובה
  error => {
    console.error("API Request Error: ", error.response ? error.response.data : error.message);
    return Promise.reject(error); // מחזירים את השגיאה כדי שנוכל לטפל בה במקום אחר
  }
);

export default {
  // get
  getTasks: async () => {
    const result = await axios.get(`/items`)    
    return result.data;
  },

  // post
  addTask: async (name) => {
    console.log('addTask', name)  
    const result = await axios.post('/items', { name: name, isComplete: false }); // יוצרת משימה חדשה עם שם לא פעיל
    return result.data;
  },

  // put
// עדכון סטטוס ה-completion של משימה (ועדכון שם אם צריך)
setCompleted: async (id, name, isComplete) => {
  if (!id || name === undefined || isComplete === undefined) {
    console.error("Missing parameters:", { id, name, isComplete });
    return;
  }

  const todoToUpdate = {
    name: name,      // שם המשימה
    isComplete: isComplete, // סטטוס ההשלמה
  };

  try {
    const result = await axios.put(`/items/${id}`, todoToUpdate);
    return result.data;
  } catch (error) {
    console.error('Error updating todo:', error);
    throw error;
  }
},  // delete
  deleteTask: async (id) => {
    console.log('deleteTask')
    const result = await axios.delete(`/items/${id}`)
    return result.data;
  }
};