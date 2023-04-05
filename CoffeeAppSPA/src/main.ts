import { createApp } from 'vue'
import './style.scss'
import '@fortawesome/fontawesome-free/css/all.css'
import App from './App.vue'

// Vuetify
import 'vuetify/styles'
import { aliases, fa } from 'vuetify/iconsets/fa'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'

//router 
import router from "./router";
import { SearchApi } from './api/SearchApi'

const vuetify = createVuetify({
  components,
  directives,
  icons: {
    defaultSet: 'fa',
    aliases,
    sets: {
      fa,
    },
  },
})
const searchApi = new SearchApi('http://localhost:5263');


createApp(App)
  .provide("SearchAPI", searchApi)
  .use(vuetify)
  .use(router)
  .mount('#app')
