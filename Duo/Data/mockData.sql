-- =============================================
-- INSERT Users (IF NOT EXISTS)
-- =============================================
INSERT INTO Users (username)
SELECT username
FROM (VALUES 
	('Andrei'),
	('Ion'),
	('Maria'),
	('Alex'),
	('Elena'),
	('Gabriel'),
	('Sofia'),
	('Matei'),
	('Ana'),
	('David'),
	('Ioana'),
	('Luca'),
	('Bianca'),
	('Victor'),
	('Cristi')
) AS source (username)
WHERE NOT EXISTS (
	SELECT 1 
	FROM Users 
	WHERE Users.username = source.username
);

	
-- =============================================
-- INSERT Hashtags (IF NOT EXISTS)
-- =============================================
INSERT INTO Hashtags (Tag)
SELECT Tag
FROM (VALUES 
	('Geography'),
	('CulturalGeography'),
	('PhysicalGeography'),
	('PlateTectonics'),
	('ClimateChange'),
	('Geopolitics'),
	('Travel'),
	('Adventure'),
	('LessonHelp'),
	('Announcements'),
	('Discover'),
	('OffTopic'),
	('UrbanPlanning'),
	('Geospatial'),
	('Cartography'),
	('GIS'),
	('RemoteSensing'),
	('Sustainability'),
	('CulturalHeritage'),
	('PopulationDynamics')
) AS source (Tag)
WHERE NOT EXISTS (
	SELECT 1 
	FROM Hashtags 
	WHERE Hashtags.Tag = source.Tag
);


BEGIN TRY
    BEGIN TRANSACTION;

	-- Check and update Posts.Title if needed
    IF COL_LENGTH('Posts', 'Title') < 200
    BEGIN
        ALTER TABLE Posts
        ALTER COLUMN Title NVARCHAR(200) NOT NULL;
    END

    -- Check and update Comments.Content if needed
    IF COL_LENGTH('Comments', 'Content') < 2000
    BEGIN
        ALTER TABLE Comments
        ALTER COLUMN Content NVARCHAR(2000) NOT NULL;
    END

    -- Declare variables to hold IDs
    DECLARE @PostID INT;
    DECLARE @CommentID INT;

	--General-Discussion

    -- =============================================
    -- POST 1: Rising Tides: The Future of Coastal Urban Centers
    -- =============================================
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Rising Tides: The Future of Coastal Urban Centers')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Rising Tides: The Future of Coastal Urban Centers',
            CONCAT(
                '## Sea-Level Rise and Urban Vulnerability  ', CHAR(13), CHAR(10),
                '*Key points:*  ', CHAR(13), CHAR(10),
                '- Projected sea-level rise by 2100: 0.3-2.5 meters  ', CHAR(13), CHAR(10),
                '- Cities at risk: Miami, Venice, Dhaka, Jakarta  ', CHAR(13), CHAR(10),
                '- Adaptation strategies:  ', CHAR(13), CHAR(10),
                '  1. Sea walls and barriers  ', CHAR(13), CHAR(10),
                '  2. Floating architecture  ', CHAR(13), CHAR(10),
                '  3. Managed retreat  ', CHAR(13), CHAR(10),
                '  ', CHAR(13), CHAR(10),
                '### Economic Impacts:  ', CHAR(13), CHAR(10),
                '| City    | Potential GDP Loss by 2050 |  ', CHAR(13), CHAR(10),
                '|---------|----------------------------|  ', CHAR(13), CHAR(10),
                '| Miami   | $3.5 billion               |  ', CHAR(13), CHAR(10),
                '| Jakarta | $10 billion                |  ', CHAR(13), CHAR(10),
                '  ', CHAR(13), CHAR(10),
                'How should cities prioritize their adaptation efforts?'
            ),
            2,
            1
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'ClimateChange')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'UrbanPlanning')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'Sustainability'));
        
        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('What are the most vulnerable cities?', 3, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Miami is already experiencing ''sunny day flooding.''', 4, @PostID, @CommentID, 2);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Interesting, I thought Venice was the worst case.', 5, @PostID, @CommentID, 3);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Are there successful adaptation examples?', 6, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('The Netherlands has advanced flood management.', 7, @PostID, @CommentID, 2);
    END

    -- =============================================
    -- POST 2: Renewable Energy: Path to 100%?
    -- =============================================
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Renewable Energy: Path to 100%?')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Renewable Energy: Path to 100%?',
            CONCAT(
                '## The Renewable Revolution  ', CHAR(13), CHAR(10),
                '*Debate points:*  ', CHAR(13), CHAR(10),
                '- Global renewable energy share: 29%  ', CHAR(13), CHAR(10),
                '- Challenges to 100% renewables:  ', CHAR(13), CHAR(10),
                '  1. Intermittency  ', CHAR(13), CHAR(10),
                '  2. Storage technology  ', CHAR(13), CHAR(10),
                '  3. Grid infrastructure  ', CHAR(13), CHAR(10),
                '  ', CHAR(13), CHAR(10),
                '### Country Comparisons:  ', CHAR(13), CHAR(10),
                '| Country | Renewable % | Target Year |  ', CHAR(13), CHAR(10),
                '|---------|-------------|-------------|  ', CHAR(13), CHAR(10),
                '| Iceland | 100%        | Achieved    |  ', CHAR(13), CHAR(10),
                '| Germany | 46%         | 2050        |  ', CHAR(13), CHAR(10),
                '  ', CHAR(13), CHAR(10),
                'Is 100% renewable feasible by 2050?'
            ),
            3,
            1
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'ClimateChange')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'Sustainability')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'Geopolitics'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('What about nuclear energy?', 8, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Nuclear has waste and safety issues.', 9, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Battery tech seems key.', 10, @PostID, 1);
    END

    -- =============================================
    -- POST 3: Water Wars: Geopolitics of Scarcity
    -- =============================================
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Water Wars: Geopolitics of Scarcity')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Water Wars: Geopolitics of Scarcity',
            CONCAT(
                '## The Blue Gold  ', CHAR(13), CHAR(10),
                '*Key issues:*  ', CHAR(13), CHAR(10),
                '- 2.2 billion lack safe water  ', CHAR(13), CHAR(10),
                '- Conflicts: Nile, Indus  ', CHAR(13), CHAR(10),
                '  ', CHAR(13), CHAR(10),
                '### Solutions:  ', CHAR(13), CHAR(10),
                '- Water treaties  ', CHAR(13), CHAR(10),
                '- Desalination  ', CHAR(13), CHAR(10),
                'Can diplomacy prevent water wars?'
            ),
            4,
            1
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'Geopolitics')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'ClimateChange'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Is desalination viable?', 11, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('It�s energy-intensive but improving.', 12, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('What about water trade?', 13, @PostID, 1);
    END

    -- =============================================
    -- POST 4: Saving Languages: Cultural Geography
    -- =============================================
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Saving Languages: Cultural Geography')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Saving Languages: Cultural Geography',
            CONCAT(
                '## Linguistic Diversity  ', CHAR(13), CHAR(10),
                '*Facts:*  ', CHAR(13), CHAR(10),
                '- 7,000+ languages worldwide  ', CHAR(13), CHAR(10),
                '- 40% at risk  ', CHAR(13), CHAR(10),
                '  ', CHAR(13), CHAR(10),
                '### Efforts:  ', CHAR(13), CHAR(10),
                '- UNESCO programs  ', CHAR(13), CHAR(10),
                'Why preserve languages?'
            ),
            5,
            1
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'CulturalGeography')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'CulturalHeritage'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Language ties to identity.', 14, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Totally, it�s our heritage!', 1, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Tech can help, right?', 1, @PostID, 1);
    END

    -- =============================================
    -- POST 5: Living with Tectonics
    -- =============================================
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Living with Tectonics')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Living with Tectonics',
            CONCAT(
                '## The Dynamic Earth  ', CHAR(13), CHAR(10),
                '*Science:*  ', CHAR(13), CHAR(10),
                '- Plate tectonics  ', CHAR(13), CHAR(10),
                '- Seismic zones  ', CHAR(13), CHAR(10),
                '  ', CHAR(13), CHAR(10),
                '### Preparedness:  ', CHAR(13), CHAR(10),
                '- Warning systems  ', CHAR(13), CHAR(10),
                'How to prepare better?'
            ),
            6,
            1
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'PhysicalGeography')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'PlateTectonics'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Japan�s preparedness is top-notch.', 2, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Still faces tsunami risks.', 3, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Can we predict quakes?', 4, @PostID, 1);
    END

    -- =============================================
    -- POST 6: Urbanization in the Global South
    -- =============================================
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Urbanization in the Global South')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Urbanization in the Global South',
            CONCAT(
                '## Rapid Urban Growth  ', CHAR(13), CHAR(10),
                '*Stats:*  ', CHAR(13), CHAR(10),
                '- 90% urban growth here  ', CHAR(13), CHAR(10),
                '- 1 billion in slums  ', CHAR(13), CHAR(10),
                '  ', CHAR(13), CHAR(10),
                '### Challenges:  ', CHAR(13), CHAR(10),
                '- Infrastructure gaps  ', CHAR(13), CHAR(10),
                'Sustainable models?'
            ),
            7,
            1
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'UrbanPlanning')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'Sustainability'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Community planning is key.', 5, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Yes, involve locals!', 6, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Smart cities viable?', 8, @PostID, 1);
    END

    -- =============================================
    -- POST 7: GIS: Mapping Our World
    -- =============================================
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'GIS: Mapping Our World')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'GIS: Mapping Our World',
            CONCAT(
                '## Geospatial Tech  ', CHAR(13), CHAR(10),
                '*Uses:*  ', CHAR(13), CHAR(10),
                '- Urban planning  ', CHAR(13), CHAR(10),
                '- Disaster response  ', CHAR(13), CHAR(10),
                '  ', CHAR(13), CHAR(10),
                '### Tools:  ', CHAR(13), CHAR(10),
                '- ArcGIS, QGIS  ', CHAR(13), CHAR(10),
                'GIS impact?'
            ),
            9,
            1
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'GIS')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'Geospatial'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('GIS is a game-changer.', 10, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Totally agree!', 11, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Open-source options?', 12, @PostID, 1);
    END

    -- =============================================
    -- POST 8: Sustainable Tourism: Balancing Act
    -- =============================================
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Sustainable Tourism: Balancing Act')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Sustainable Tourism: Balancing Act',
            CONCAT(
                '## Tourism & Environment  ', CHAR(13), CHAR(10),
                '*Impacts:*  ', CHAR(13), CHAR(10),
                '- Carbon footprint  ', CHAR(13), CHAR(10),
                '- Overtourism  ', CHAR(13), CHAR(10),
                '  ', CHAR(13), CHAR(10),
                '### Solutions:  ', CHAR(13), CHAR(10),
                '- Eco-tourism  ', CHAR(13), CHAR(10),
                'More sustainable?'
            ),
            10,
            1
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'Travel')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'Sustainability'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Locals must benefit.', 13, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Agreed, keep profits local.', 14, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Virtual tourism?', 1, @PostID, 1);
    END

    -- =============================================
    -- POST 9: Migration Trends: Global Perspective
    -- =============================================
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Migration Trends: Global Perspective')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Migration Trends: Global Perspective',
            CONCAT(
                '## Human Mobility  ', CHAR(13), CHAR(10),
                '*Trends:*  ', CHAR(13), CHAR(10),
                '- 281M migrants (2020)  ', CHAR(13), CHAR(10),
                '- Climate migration rising  ', CHAR(13), CHAR(10),
                '  ', CHAR(13), CHAR(10),
                '### Factors:  ', CHAR(13), CHAR(10),
                '- Economy, conflict  ', CHAR(13), CHAR(10),
                'Policy solutions?'
            ),
            11,
            1
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'PopulationDynamics')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'Geopolitics'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Migration is complex.', 1, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Needs diverse solutions.', 2, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Cultural impacts?', 3, @PostID, 1);
    END

    -- =============================================
    -- POST 10: Food Security in a Changing Climate
    -- =============================================
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Food Security in a Changing Climate')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Food Security in a Changing Climate',
            CONCAT(
                '## Feeding the World  ', CHAR(13), CHAR(10),
                '*Challenges:*  ', CHAR(13), CHAR(10),
                '- Climate impacts on crops  ', CHAR(13), CHAR(10),
                '- Soil degradation  ', CHAR(13), CHAR(10),
                '  ', CHAR(13), CHAR(10),
                '### Innovations:  ', CHAR(13), CHAR(10),
                '- Vertical farming  ', CHAR(13), CHAR(10),
                'Food for all?'
            ),
            12,
            1
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'ClimateChange')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'Sustainability'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Agroecology could help.', 4, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Yes, nature-friendly farming!', 5, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Reduce waste too?', 6, @PostID, 1);
    END

	--Lesson-Help
	IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Lat & Long Basics')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Lat & Long Basics',
            CONCAT(
                '## Understanding Coordinates', CHAR(13), CHAR(10),
                'Latitude and longitude are used to locate any point on Earth.', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Key Points:', CHAR(13), CHAR(10),
                '- **Latitude**: Measures north-south (0� at Equator, 90� at poles).', CHAR(13), CHAR(10),
                '- **Longitude**: Measures east-west (0� at Prime Meridian, 180� at International Date Line).', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Example:', CHAR(13), CHAR(10),
                '| Location   | Latitude | Longitude |', CHAR(13), CHAR(10),
                '|------------|----------|-----------|', CHAR(13), CHAR(10),
                '| New York   | 40.7128� N | 74.0060� W |', CHAR(13), CHAR(10),
                '| Sydney     | 33.8688� S | 151.2093� E |', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                'How do you read coordinates on a map?'
            ),
            1,
            2
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'Geography')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'Cartography'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Does longitude affect time?', 2, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Yes, every 15� of longitude equals 1 hour of time difference.', 3, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('What�s the easiest way to remember lat vs long?', 4, @PostID, 1);
    END

    -- **Post 2: Volcanoes: Types and Formation**
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Volcano Types')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Volcano Types',
            CONCAT(
                '## Exploring Volcanoes', CHAR(13), CHAR(10),
                'Volcanoes form where tectonic plates interact or magma escapes.', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Types of Volcanoes:', CHAR(13), CHAR(10),
                '- **Shield**: Broad, gentle slopes (e.g., Mauna Loa).', CHAR(13), CHAR(10),
                '- **Stratovolcano**: Steep, explosive (e.g., Mount Fuji).', CHAR(13), CHAR(10),
                '- **Cinder Cone**: Small, steep-sided (e.g., Paricutin).', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Formation:', CHAR(13), CHAR(10),
                '| Type         | Cause                  |', CHAR(13), CHAR(10),
                '|--------------|------------------------|', CHAR(13), CHAR(10),
                '| Shield       | Low-viscosity lava     |', CHAR(13), CHAR(10),
                '| Stratovolcano| High-viscosity lava    |', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                'What causes some volcanoes to be more explosive?'
            ),
            5,
            2
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'PlateTectonics')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'PhysicalGeography'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Why are stratovolcanoes so dangerous?', 6, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Their thick lava traps gases, leading to explosive eruptions.', 7, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Are there active volcanoes near me?', 8, @PostID, 1);
    END

    -- **Post 3: Biomes: Earth�s Ecosystems**
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Biome Basics')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Biome Basics',
            CONCAT(
                '## What Are Biomes?', CHAR(13), CHAR(10),
                'Biomes are large regions defined by climate, flora, and fauna.', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Major Biomes:', CHAR(13), CHAR(10),
                '- **Tundra**: Cold, treeless, permafrost.', CHAR(13), CHAR(10),
                '- **Rainforest**: Warm, wet, biodiverse.', CHAR(13), CHAR(10),
                '- **Desert**: Dry, extreme temperatures.', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Examples:', CHAR(13), CHAR(10),
                '| Biome      | Location         |', CHAR(13), CHAR(10),
                '|------------|------------------|', CHAR(13), CHAR(10),
                '| Tundra     | Arctic           |', CHAR(13), CHAR(10),
                '| Rainforest | Amazon Basin     |', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                'How do biomes affect biodiversity?'
            ),
            9,
            2
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'PhysicalGeography')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'Sustainability'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('What�s the difference between a biome and an ecosystem?', 10, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('A biome is a large region; an ecosystem is a smaller, specific community.', 11, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Which biome has the most species?', 12, @PostID, 1);
    END

    -- **Post 4: Urbanization: Growth of Cities**
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Urbanization 101')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Urbanization 101',
            CONCAT(
                '## Rise of Urban Areas', CHAR(13), CHAR(10),
                'Urbanization is the shift from rural to urban living.', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Causes:', CHAR(13), CHAR(10),
                '- **Industrialization**: Job opportunities in cities.', CHAR(13), CHAR(10),
                '- **Migration**: People seeking better services.', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Impacts:', CHAR(13), CHAR(10),
                '| Aspect     | Effect            |', CHAR(13), CHAR(10),
                '|------------|-------------------|', CHAR(13), CHAR(10),
                '| Population | Density increases |', CHAR(13), CHAR(10),
                '| Environment| Pollution rises   |', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                'What are the pros and cons of urbanization?'
            ),
            13,
            2
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'CulturalGeography')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'PopulationDynamics'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('How does urbanization affect rural areas?', 14, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Rural areas may lose population and resources.', 1, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Are megacities sustainable?', 1, @PostID, 1);
    END

    -- **Post 5: Glaciers: Formation and Movement**
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Glacier Dynamics')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Glacier Dynamics',
            CONCAT(
                '## Ice on the Move', CHAR(13), CHAR(10),
                'Glaciers form from compacted snow and shape landscapes.', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Formation:', CHAR(13), CHAR(10),
                '- Snow accumulates and compresses into ice.', CHAR(13), CHAR(10),
                '- Requires cold climate and high snowfall.', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Movement:', CHAR(13), CHAR(10),
                '| Type       | Description         |', CHAR(13), CHAR(10),
                '|------------|---------------------|', CHAR(13), CHAR(10),
                '| Internal   | Ice deforms         |', CHAR(13), CHAR(10),
                '| Basal      | Slides over ground  |', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                'How do glaciers carve valleys?'
            ),
            2,
            2
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'PhysicalGeography')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'ClimateChange'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Are glaciers shrinking due to climate change?', 3, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Yes, many are retreating rapidly.', 4, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('What�s the biggest glacier in the world?', 5, @PostID, 1);
    END

    -- **Post 6: Migration Patterns Explained**
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Migration Patterns')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Migration Patterns',
            CONCAT(
                '## Why People Move', CHAR(13), CHAR(10),
                'Migration shapes populations and cultures globally.', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Types:', CHAR(13), CHAR(10),
                '- **Forced**: Refugees fleeing conflict.', CHAR(13), CHAR(10),
                '- **Voluntary**: Seeking jobs or education.', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Examples:', CHAR(13), CHAR(10),
                '| Type       | Example             |', CHAR(13), CHAR(10),
                '|------------|---------------------|', CHAR(13), CHAR(10),
                '| Forced     | Syrian refugees     |', CHAR(13), CHAR(10),
                '| Voluntary  | Rural-to-urban move |', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                'What drives migration in your region?'
            ),
            6,
            2
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'CulturalGeography')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'PopulationDynamics'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('How does migration affect culture?', 7, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('It can blend traditions or spark new ones.', 8, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('What�s the difference between immigration and emigration?', 9, @PostID, 1);
    END

    -- **Post 7: Deserts: Features and Life**
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Desert Ecosystems')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Desert Ecosystems',
            CONCAT(
                '## Life in the Dry', CHAR(13), CHAR(10),
                'Deserts are arid regions with unique adaptations.', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Features:', CHAR(13), CHAR(10),
                '- **Hot Deserts**: High daytime temps (e.g., Sahara).', CHAR(13), CHAR(10),
                '- **Cold Deserts**: Freezing nights (e.g., Gobi).', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Adaptations:', CHAR(13), CHAR(10),
                '| Organism   | Adaptation         |', CHAR(13), CHAR(10),
                '|------------|--------------------|', CHAR(13), CHAR(10),
                '| Cactus     | Stores water       |', CHAR(13), CHAR(10),
                '| Camel      | Hump for fat       |', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                'How do plants survive in deserts?'
            ),
            10,
            2
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'PhysicalGeography'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('What�s the largest desert?', 11, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Antarctica, technically�it�s a cold desert!', 12, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('How do animals cope with no water?', 13, @PostID, 1);
    END

    -- **Post 8: Ocean Currents: Global Flow**
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Ocean Currents')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Ocean Currents',
            CONCAT(
                '## Water on the Move', CHAR(13), CHAR(10),
                'Ocean currents distribute heat and nutrients worldwide.', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Types:', CHAR(13), CHAR(10),
                '- **Surface Currents**: Driven by wind (e.g., Gulf Stream).', CHAR(13), CHAR(10),
                '- **Deep Currents**: Driven by density (e.g., Thermohaline).', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Effects:', CHAR(13), CHAR(10),
                '| Current    | Impact             |', CHAR(13), CHAR(10),
                '|------------|--------------------|', CHAR(13), CHAR(10),
                '| Gulf Stream| Warms Western Europe |', CHAR(13), CHAR(10),
                '| El Ni�o    | Alters weather     |', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                'How do currents affect climate?'
            ),
            14,
            2
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'PhysicalGeography')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'ClimateChange'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('What causes ocean currents?', 5, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Wind, temperature, and salinity differences drive them.', 1, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('How does El Ni�o change rainfall?', 2, @PostID, 1);
    END

    -- **Post 9: Landforms: Shaping Earth**
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Landform Lessons')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Landform Lessons',
            CONCAT(
                '## Earth�s Features', CHAR(13), CHAR(10),
                'Landforms are natural shapes on Earth�s surface.', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Types:', CHAR(13), CHAR(10),
                '- **Mountains**: Tectonic uplift (e.g., Alps).', CHAR(13), CHAR(10),
                '- **Plains**: Flat, depositional (e.g., Great Plains).', CHAR(13), CHAR(10),
                '- **Plateaus**: Elevated flatlands (e.g., Tibetan Plateau).', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Formation:', CHAR(13), CHAR(10),
                '| Landform  | Process            |', CHAR(13), CHAR(10),
                '|-----------|--------------------|', CHAR(13), CHAR(10),
                '| Mountains | Plate collision    |', CHAR(13), CHAR(10),
                '| Plains    | Sediment deposit   |', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                'What�s your favorite landform?'
            ),
            3,
            2
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'PhysicalGeography')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'PlateTectonics'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('How long does it take to form a mountain?', 4, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Millions of years through tectonic activity.', 5, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Are plateaus good for farming?', 6, @PostID, 1);
    END

    -- **Post 10: Economic Geography Basics**
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Economic Geography')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Economic Geography',
            CONCAT(
                '## Resources and Trade', CHAR(13), CHAR(10),
                'Economic geography studies how location affects economies.', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Key Concepts:', CHAR(13), CHAR(10),
                '- **Primary**: Extraction (e.g., mining).', CHAR(13), CHAR(10),
                '- **Secondary**: Manufacturing (e.g., factories).', CHAR(13), CHAR(10),
                '- **Tertiary**: Services (e.g., retail).', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                '### Examples:', CHAR(13), CHAR(10),
                '| Sector    | Region             |', CHAR(13), CHAR(10),
                '|-----------|--------------------|', CHAR(13), CHAR(10),
                '| Primary   | Oil-rich Middle East |', CHAR(13), CHAR(10),
                '| Tertiary  | Silicon Valley     |', CHAR(13), CHAR(10),
                CHAR(13), CHAR(10),
                'How does geography influence your local economy?'
            ),
            7,
            2
        );
        
        SET @PostID = SCOPE_IDENTITY();
        
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'CulturalGeography')),
            (@PostID, (SELECT Id FROM Hashtags WHERE Tag = 'Sustainability'));
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Why do some regions specialize in one sector?', 8, @PostID, 1);
        
        SET @CommentID = SCOPE_IDENTITY();
        
        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('It depends on resources and infrastructure.', 9, @PostID, @CommentID, 2);
        
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('How does trade connect economic geography?', 10, @PostID, 1);
    END

	--Off-topic

	-- Post 1: Coastal Cities at Risk
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Coastal Cities at Risk')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Coastal Cities at Risk',
            CONCAT(
                '## Rising Sea Levels Threaten Urban Centers  ', CHAR(13), CHAR(10),
                '*Key points:*  ', CHAR(13), CHAR(10),
                '- Global sea level rise: 0.3-1.2m by 2100  ', CHAR(13), CHAR(10),
                '- Case studies:  ', CHAR(13), CHAR(10),
                '  1. Miami: Frequent flooding  ', CHAR(13), CHAR(10),
                '  2. Jakarta: Sinking city  ', CHAR(13), CHAR(10),
                '  3. Venice: MOSE barriers  ', CHAR(13), CHAR(10),
                '### Mitigation Strategies:  ', CHAR(13), CHAR(10),
                '| Approach      | Effectiveness | Cost |  ', CHAR(13), CHAR(10),
                '|---------------|---------------|------|  ', CHAR(13), CHAR(10),
                '| Sea Walls     | Medium        | High |  ', CHAR(13), CHAR(10),
                '| Mangroves     | High          | Low  |  ', CHAR(13), CHAR(10),
                'How can cities adapt?'
            ),
            2,  -- UserID
            3   
        );

        DECLARE @CoastalPostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@CoastalPostID, (SELECT Id FROM Hashtags WHERE Tag = 'ClimateChange')),
            (@CoastalPostID, (SELECT Id FROM Hashtags WHERE Tag = 'UrbanPlanning'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('What about mangroves?', 3, @CoastalPostID, 1);

        DECLARE @MangroveCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('They�re natural barriers!', 4, @CoastalPostID, @MangroveCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Are they enough though?', 5, @CoastalPostID, 1);
    END

    -- Post 2: Mapping Language Diversity
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Mapping Language Diversity')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Mapping Language Diversity',
            CONCAT(
                '## Language and Culture  ', CHAR(13), CHAR(10),
                '*Highlights:*  ', CHAR(13), CHAR(10),
                '- Maps reveal history  ', CHAR(13), CHAR(10),
                '- Examples:  ', CHAR(13), CHAR(10),
                '  1. India: 22 languages  ', CHAR(13), CHAR(10),
                '  2. Switzerland: 4 languages  ', CHAR(13), CHAR(10),
                '### Techniques:  ', CHAR(13), CHAR(10),
                '| Method       | Pros          | Cons          |  ', CHAR(13), CHAR(10),
                '|--------------|---------------|---------------|  ', CHAR(13), CHAR(10),
                '| Choropleth   | Easy to read  | Oversimplifies|  ', CHAR(13), CHAR(10),
                '| Dot Density  | Detailed      | Cluttered     |  ', CHAR(13), CHAR(10),
                'What can maps teach us?'
            ),
            3,  -- UserID
            3   -- CategoryID: General-Discussion
        );

        DECLARE @LanguagePostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@LanguagePostID, (SELECT Id FROM Hashtags WHERE Tag = 'CulturalGeography')),
            (@LanguagePostID, (SELECT Id FROM Hashtags WHERE Tag = 'Cartography'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('How to map dialects?', 6, @LanguagePostID, 1);

        DECLARE @DialectCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('As subgroups, usually.', 7, @LanguagePostID, @DialectCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Any software tips?', 8, @LanguagePostID, 1);
    END

    -- Post 1: Plate Tectonics 101
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Plate Tectonics 101')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Plate Tectonics 101',
            CONCAT(
                '## Earth�s Moving Plates  ', CHAR(13), CHAR(10),
                '*Basics:*  ', CHAR(13), CHAR(10),
                '- Theory explained  ', CHAR(13), CHAR(10),
                '- Boundary types:  ', CHAR(13), CHAR(10),
                '  1. Convergent: Collision  ', CHAR(13), CHAR(10),
                '  2. Divergent: Spreading  ', CHAR(13), CHAR(10),
                '  3. Transform: Sliding  ', CHAR(13), CHAR(10),
                'How do plates shape Earth?'
            ),
            4,  -- UserID
            3   -- CategoryID: Lesson-Help
        );

        DECLARE @PlatePostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PlatePostID, (SELECT Id FROM Hashtags WHERE Tag = 'PlateTectonics')),
            (@PlatePostID, (SELECT Id FROM Hashtags WHERE Tag = 'LessonHelp'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Convergent vs divergent?', 9, @PlatePostID, 1);

        DECLARE @BoundaryCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Collide vs apart!', 10, @PlatePostID, @BoundaryCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Video resources?', 11, @PlatePostID, 1);
    END

    -- Post 2: Getting Started with GIS
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Getting Started with GIS')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Getting Started with GIS',
            CONCAT(
                '## GIS for Beginners  ', CHAR(13), CHAR(10),
                '*Intro:*  ', CHAR(13), CHAR(10),
                '- What is GIS?  ', CHAR(13), CHAR(10),
                '- Tools:  ', CHAR(13), CHAR(10),
                '  1. QGIS: Free, open-source  ', CHAR(13), CHAR(10),
                '  2. ArcGIS: Industry standard  ', CHAR(13), CHAR(10),
                'Try a simple project!'
            ),
            5,  -- UserID
            3   -- CategoryID: Lesson-Help
        );

        DECLARE @GISPostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@GISPostID, (SELECT Id FROM Hashtags WHERE Tag = 'GIS')),
            (@GISPostID, (SELECT Id FROM Hashtags WHERE Tag = 'LessonHelp'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Best GIS for beginners?', 12, @GISPostID, 1);

        DECLARE @GISCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('QGIS�free and easy!', 13, @GISPostID, @GISCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Tutorials anywhere?', 14, @GISPostID, 1);
    END

    -- Post 1: Must-Visit Places
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Must-Visit Places')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Must-Visit Places',
            CONCAT(
                '## Geographer�s Travel List  ', CHAR(13), CHAR(10),
                '*Destinations:*  ', CHAR(13), CHAR(10),
                '- Iceland: Volcanic wonders  ', CHAR(13), CHAR(10),
                '- Silk Road: Historic routes  ', CHAR(13), CHAR(10),
                'Where�s your next trip?'
            ),
            6,  -- UserID
            3   -- CategoryID: Off-topic
        );

        DECLARE @TravelPostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@TravelPostID, (SELECT Id FROM Hashtags WHERE Tag = 'Travel')),
            (@TravelPostID, (SELECT Id FROM Hashtags WHERE Tag = 'Adventure'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Atacama Desert?', 7, @TravelPostID, 1);

        DECLARE @DesertCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Yes, super dry!', 8, @TravelPostID, @DesertCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Urban spots?', 9, @TravelPostID, 1);
    END

    -- Post 2: Geography in Movies
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Geography in Movies')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Geography in Movies',
            CONCAT(
                '## Geography on Screen  ', CHAR(13), CHAR(10),
                '*Examples:*  ', CHAR(13), CHAR(10),
                '- Inception: Cityscapes  ', CHAR(13), CHAR(10),
                '- Planet Earth: Nature  ', CHAR(13), CHAR(10),
                'What�s your fave?'
            ),
            10,  -- UserID
            3   -- CategoryID: Off-topic
        );

        DECLARE @MoviesPostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@MoviesPostID, (SELECT Id FROM Hashtags WHERE Tag = 'OffTopic')),
            (@MoviesPostID, (SELECT Id FROM Hashtags WHERE Tag = 'CulturalGeography'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Game of Thrones map!', 11, @MoviesPostID, 1);

        DECLARE @GOTCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Westeros is epic!', 12, @MoviesPostID, @GOTCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Board games?', 13, @MoviesPostID, 1);
    END


    -- Post 1: Lost City in Amazon
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Lost City in Amazon')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Lost City in Amazon',
            CONCAT(
                '## Archaeological Find  ', CHAR(13), CHAR(10),
                '*Details:*  ', CHAR(13), CHAR(10),
                '- LiDAR reveals cities  ', CHAR(13), CHAR(10),
                '- Amazon rainforest  ', CHAR(13), CHAR(10),
                'What else is hidden?'
            ),
            14,  -- UserID
            3   -- CategoryID: Discover
        );

        DECLARE @AmazonPostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@AmazonPostID, (SELECT Id FROM Hashtags WHERE Tag = 'Discover')),
            (@AmazonPostID, (SELECT Id FROM Hashtags WHERE Tag = 'CulturalHeritage'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('How does LiDAR work?', 1, @AmazonPostID, 1);

        DECLARE @LiDARCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Penetrates trees!', 1, @AmazonPostID, @LiDARCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Virtual tours?', 2, @AmazonPostID, 1);
    END

    -- Post 2: AI in Geospatial
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'AI in Geospatial')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'AI in Geospatial',
            CONCAT(
                '## Tech Meets Geography  ', CHAR(13), CHAR(10),
                '*Uses:*  ', CHAR(13), CHAR(10),
                '- Land use analysis  ', CHAR(13), CHAR(10),
                '- Disaster prediction  ', CHAR(13), CHAR(10),
                'Future of geospatial?'
            ),
            3,  -- UserID
            3   -- CategoryID: Discover
        );

        DECLARE @AIPostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@AIPostID, (SELECT Id FROM Hashtags WHERE Tag = 'Geospatial')),
            (@AIPostID, (SELECT Id FROM Hashtags WHERE Tag = 'GIS'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('AI tools available?', 4, @AIPostID, 1);

        DECLARE @AIToolsCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Try TensorFlow!', 5, @AIPostID, @AIToolsCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Disaster accuracy?', 6, @AIPostID, 1);
    END

    -- Post 1: AAG Meeting 2024
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'AAG Meeting 2024')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'AAG Meeting 2024',
            CONCAT(
                '## Geography Conference  ', CHAR(13), CHAR(10),
                '*Details:*  ', CHAR(13), CHAR(10),
                '- Date: April 2024  ', CHAR(13), CHAR(10),
                '- Call for papers now!  ', CHAR(13), CHAR(10),
                'Join us there?'
            ),
            7,  -- UserID
            3   -- CategoryID: Announcements
        );

        DECLARE @AAGPostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@AAGPostID, (SELECT Id FROM Hashtags WHERE Tag = 'Announcements')),
            (@AAGPostID, (SELECT Id FROM Hashtags WHERE Tag = 'Geopolitics'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Virtual option?', 8, @AAGPostID, 1);

        DECLARE @VirtualCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Yes, hybrid!', 9, @AAGPostID, @VirtualCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Theme this year?', 10, @AAGPostID, 1);
    END

    -- Post 2: GIS Analyst Job
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'GIS Analyst Job')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'GIS Analyst Job',
            CONCAT(
                '## Job Opportunity  ', CHAR(13), CHAR(10),
                '*Details:*  ', CHAR(13), CHAR(10),
                '- 3-5 yrs experience  ', CHAR(13), CHAR(10),
                '- Environmental firm  ', CHAR(13), CHAR(10),
                'Apply now!'
            ),
            11,  -- UserID
            3   -- CategoryID: Announcements
        );

        DECLARE @JobPostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@JobPostID, (SELECT Id FROM Hashtags WHERE Tag = 'Announcements')),
            (@JobPostID, (SELECT Id FROM Hashtags WHERE Tag = 'GIS'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Experience needed?', 12, @JobPostID, 1);

        DECLARE @ExpCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Mid-level, 3-5 yrs.', 13, @JobPostID, @ExpCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Remote work?', 14, @JobPostID, 1);
    END

	--Discover
	 -- Post 1: Volcanoes of Mars
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Volcanoes of Mars')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Volcanoes of Mars',
            CONCAT(
                '## Red Planet Giants  ', CHAR(13), CHAR(10),
                '*Highlights:*  ', CHAR(13), CHAR(10),
                '- Olympus Mons: 22 km high  ', CHAR(13), CHAR(10),
                '- Shield volcanoes dominate  ', CHAR(13), CHAR(10),
                'What can we learn?'
            ),
            1,  -- UserID
            4   -- CategoryID: Discover
        );

        DECLARE @MarsPostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@MarsPostID, (SELECT Id FROM Hashtags WHERE Tag = 'Discover')),
            (@MarsPostID, (SELECT Id FROM Hashtags WHERE Tag = 'PhysicalGeography'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('How tall compared to Earth?', 2, @MarsPostID, 1);

        DECLARE @MarsCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('3x Everest!', 3, @MarsPostID, @MarsCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Active volcanoes?', 4, @MarsPostID, 1);
    END

    -- Post 2: Deep Ocean Trenches
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Deep Ocean Trenches')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Deep Ocean Trenches',
            CONCAT(
                '## Earth�s Hidden Depths  ', CHAR(13), CHAR(10),
                '*Key Facts:*  ', CHAR(13), CHAR(10),
                '- Mariana Trench: 11 km  ', CHAR(13), CHAR(10),
                '- Subduction zones  ', CHAR(13), CHAR(10),
                'What lives down there?'
            ),
            5,  -- UserID
            4   -- CategoryID: Discover
        );

        DECLARE @OceanPostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@OceanPostID, (SELECT Id FROM Hashtags WHERE Tag = 'Discover')),
            (@OceanPostID, (SELECT Id FROM Hashtags WHERE Tag = 'PhysicalGeography')),
            (@OceanPostID, (SELECT Id FROM Hashtags WHERE Tag = 'PlateTectonics'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Any new species found?', 6, @OceanPostID, 1);

        DECLARE @SpeciesCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Yes, weird fish!', 7, @OceanPostID, @SpeciesCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('How do they map it?', 8, @OceanPostID, 1);
    END

    -- Post 3: Sahara�s Lost Rivers
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Sahara�s Lost Rivers')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Sahara�s Lost Rivers',
            CONCAT(
                '## Ancient Waterways  ', CHAR(13), CHAR(10),
                '*Discovery:*  ', CHAR(13), CHAR(10),
                '- Radar shows old rivers  ', CHAR(13), CHAR(10),
                '- Once lush Sahara  ', CHAR(13), CHAR(10),
                'Climate clues?'
            ),
            9,  -- UserID
            4   -- CategoryID: Discover
        );

        DECLARE @SaharaPostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@SaharaPostID, (SELECT Id FROM Hashtags WHERE Tag = 'Discover')),
            (@SaharaPostID, (SELECT Id FROM Hashtags WHERE Tag = 'ClimateChange'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('How old are these?', 10, @SaharaPostID, 1);

        DECLARE @SaharaCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('10,000+ years!', 11, @SaharaPostID, @SaharaCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('More deserts like this?', 12, @SaharaPostID, 1);
    END

    -- Post 4: Ice Age Migration
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Ice Age Migration')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Ice Age Migration',
            CONCAT(
                '## Human Journeys  ', CHAR(13), CHAR(10),
                '*Findings:*  ', CHAR(13), CHAR(10),
                '- Bering Land Bridge  ', CHAR(13), CHAR(10),
                '- DNA evidence  ', CHAR(13), CHAR(10),
                'Where did we go?'
            ),
            13,  -- UserID
            4   -- CategoryID: Discover
        );

        DECLARE @IcePostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@IcePostID, (SELECT Id FROM Hashtags WHERE Tag = 'Discover')),
            (@IcePostID, (SELECT Id FROM Hashtags WHERE Tag = 'CulturalGeography'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('How did they survive?', 14, @IcePostID, 1);

        DECLARE @IceCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Adaptation, tools!', 1, @IcePostID, @IceCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Other routes?', 1, @IcePostID, 1);
    END

    -- Post 5: Antarctica�s Secrets
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Antarctica�s Secrets')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Antarctica�s Secrets',
            CONCAT(
                '## Icy Mysteries  ', CHAR(13), CHAR(10),
                '*Revelations:*  ', CHAR(13), CHAR(10),
                '- Subglacial lakes  ', CHAR(13), CHAR(10),
                '- Ancient ice cores  ', CHAR(13), CHAR(10),
                'What�s beneath?'
            ),
            2,  -- UserID
            4   -- CategoryID: Discover
        );

        DECLARE @AntarcticaPostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@AntarcticaPostID, (SELECT Id FROM Hashtags WHERE Tag = 'Discover')),
            (@AntarcticaPostID, (SELECT Id FROM Hashtags WHERE Tag = 'PhysicalGeography'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Life in those lakes?', 3, @AntarcticaPostID, 1);

        DECLARE @LakeCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Microbes, yes!', 4, @AntarcticaPostID, @LakeCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('How deep?', 5, @AntarcticaPostID, 1);
    END

    -- Post 6: New Island Birth
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'New Island Birth')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'New Island Birth',
            CONCAT(
                '## Volcanic Creation  ', CHAR(13), CHAR(10),
                '*Event:*  ', CHAR(13), CHAR(10),
                '- Hunga Tonga eruption  ', CHAR(13), CHAR(10),
                '- New land formed  ', CHAR(13), CHAR(10),
                'How long will it last?'
            ),
            6,  -- UserID
            4   -- CategoryID: Discover
        );

        DECLARE @IslandPostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@IslandPostID, (SELECT Id FROM Hashtags WHERE Tag = 'Discover')),
            (@IslandPostID, (SELECT Id FROM Hashtags WHERE Tag = 'PlateTectonics'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Erosion risk?', 7, @IslandPostID, 1);

        DECLARE @ErosionCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('High, it�s young!', 8, @IslandPostID, @ErosionCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('More examples?', 9, @IslandPostID, 1);
    END

    -- Post 7: Ancient Trade Routes
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Ancient Trade Routes')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Ancient Trade Routes',
            CONCAT(
                '## Paths of History  ', CHAR(13), CHAR(10),
                '*Routes:*  ', CHAR(13), CHAR(10),
                '- Amber Road: Europe  ', CHAR(13), CHAR(10),
                '- Incense Route: Arabia  ', CHAR(13), CHAR(10),
                'What did they carry?'
            ),
            10,  -- UserID
            4   -- CategoryID: Discover
        );

        DECLARE @TradePostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@TradePostID, (SELECT Id FROM Hashtags WHERE Tag = 'Discover')),
            (@TradePostID, (SELECT Id FROM Hashtags WHERE Tag = 'CulturalHeritage'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Goods traded?', 11, @TradePostID, 1);

        DECLARE @GoodsCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Spices, gems!', 12, @TradePostID, @GoodsCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Maps available?', 13, @TradePostID, 1);
    END

    -- Post 8: Magnetic Pole Shift
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Magnetic Pole Shift')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Magnetic Pole Shift',
            CONCAT(
                '## Earth�s Compass  ', CHAR(13), CHAR(10),
                '*Phenomenon:*  ', CHAR(13), CHAR(10),
                '- North Pole moving  ', CHAR(13), CHAR(10),
                '- Past reversals  ', CHAR(13), CHAR(10),
                'Impact on navigation?'
            ),
            14,  -- UserID
            4   -- CategoryID: Discover
        );

        DECLARE @PolePostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@PolePostID, (SELECT Id FROM Hashtags WHERE Tag = 'Discover')),
            (@PolePostID, (SELECT Id FROM Hashtags WHERE Tag = 'PhysicalGeography'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('How fast is it?', 1, @PolePostID, 1);

        DECLARE @SpeedCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('55 km/year now!', 1, @PolePostID, @SpeedCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Affects GPS?', 2, @PolePostID, 1);
    END

    -- Post 9: Caves of Altamira
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Caves of Altamira')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Caves of Altamira',
            CONCAT(
                '## Prehistoric Art  ', CHAR(13), CHAR(10),
                '*Discovery:*  ', CHAR(13), CHAR(10),
                '- Spain, 35,000 yrs  ', CHAR(13), CHAR(10),
                '- Bison paintings  ', CHAR(13), CHAR(10),
                'What do they tell us?'
            ),
            3,  -- UserID
            4   -- CategoryID: Discover
        );

        DECLARE @CavePostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@CavePostID, (SELECT Id FROM Hashtags WHERE Tag = 'Discover')),
            (@CavePostID, (SELECT Id FROM Hashtags WHERE Tag = 'CulturalHeritage'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Art or symbols?', 4, @CavePostID, 1);

        DECLARE @ArtCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Maybe both!', 5, @CavePostID, @ArtCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('More caves like this?', 6, @CavePostID, 1);
    END

    -- Post 10: Exoplanet Climates
    IF NOT EXISTS (SELECT 1 FROM Posts WHERE Title = 'Exoplanet Climates')
    BEGIN
        INSERT INTO Posts (Title, Description, UserID, CategoryID)
        VALUES (
            'Exoplanet Climates',
            CONCAT(
                '## Beyond Earth  ', CHAR(13), CHAR(10),
                '*Exploration:*  ', CHAR(13), CHAR(10),
                '- TRAPPIST-1 system  ', CHAR(13), CHAR(10),
                '- Weather models  ', CHAR(13), CHAR(10),
                'Habitable worlds?'
            ),
            7,  -- UserID
            4   -- CategoryID: Discover
        );

        DECLARE @ExoPostID INT = SCOPE_IDENTITY();

        -- Link Hashtags
        INSERT INTO PostHashtags (PostID, HashtagID)
        VALUES 
            (@ExoPostID, (SELECT Id FROM Hashtags WHERE Tag = 'Discover')),
            (@ExoPostID, (SELECT Id FROM Hashtags WHERE Tag = 'ClimateChange'));

        -- Add Comments
        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('Water possible?', 8, @ExoPostID, 1);

        DECLARE @WaterCommentID INT = SCOPE_IDENTITY();

        INSERT INTO Comments (Content, UserID, PostID, ParentCommentID, Level)
        VALUES ('Some, maybe!', 9, @ExoPostID, @WaterCommentID, 2);

        INSERT INTO Comments (Content, UserID, PostID, Level)
        VALUES ('How do we know?', 10, @ExoPostID, 1);
    END

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS VARCHAR(10));
    PRINT 'Error Message: ' + ERROR_MESSAGE();
    PRINT 'Error Severity: ' + CAST(ERROR_SEVERITY() AS VARCHAR(10));
    PRINT 'Error State: ' + CAST(ERROR_STATE() AS VARCHAR(10));
    PRINT 'Error Line: ' + CAST(ERROR_LINE() AS VARCHAR(10));
END CATCH;