﻿Vue.config.devtools = true

var app = new Vue({
    el: '#app',
    data: {
        username = ""
        }
    },
    mounted() {
        //todo: get all users
    },
    methods: {
        createUser() {
            axios.post('/users', {username: this.username})
                .then(res => {
                    console.log(res);
                    this.products = res.data;
                })
                .catch(err => {
                    console.log(err);
                })
                .then(() => {
                });
        }
    }
})