const mappingJSON = () => {
    return {
            "age_restriction": {
                "EIGHTEEN_PLUS": "18+",
                "SIXTEEN_PLUS": "16+",
                "TWELVE_PLUS": "12+",
                "SIX_PLUS": "6+"
            },
            "age_restriction_options": [
                { value: "EIGHTEEN_PLUS", label: "18+" },
                { value: "SIXTEEN_PLUS", label: "16+" },
                { value: "TWELVE_PLUS", label: "12+" },
                { value: "SIX_PLUS", label: "6+" }
            ],
            "category_options": [
                { value: "MOVIE", label: "Movie" },
                { value: "ANIMATED_SERIES", label: "Animated series" },
                { value: "SERIES", label: "Series" },
                { value: "ANIMATED_FILM", label: "Animated film" },
            ],
            "genre_options": [
                { value: "MUSICAL", label: "Musical" },
                { value: "DRAMA", label: "Drama" },
                { value: "THRILLER", label: "Thriller" },
                { value: "HORROR_FILM", label: "Horror film" },
                { value: "ACTION_FILM", label: "Action film" },
                { value: "BLOCKBUSTER", label: "Blockbuster" },
                { value: "COMEDY", label: "Comedy" },
                { value: "DOCUMENTARY", label: "Documentary" },
                { value: "CARTOON", label: "Cartoon" },
                { value: "HISTORICAL_FILM", label: "Historical film" }
            ]
        };
}

export default mappingJSON;