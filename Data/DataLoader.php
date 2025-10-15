<?php

namespace App\GameClasses;

use App\Exceptions\ErrorListException;
use App\Exceptions\TriesTo;
use App\GameClasses\Generic\ObjectRef;
use App\GameClasses\Objects\WithTypeObject;
use App\GameClasses\Objects\Game\World;
use App\GameClasses\Types\Categorized\LeaderType;
use App\GameClasses\Types\Complex\ResourceType;
use App\GameClasses\Types\TypeObject;
use App\GameClasses\Types\Upgrading\CultureType;
use Illuminate\Support\Facades\Validator;

class DataLoader
{
    /** @var callable|null */
    public static mixed $output = null;

    protected static array $classes = [
        'MajorCultureType' => CultureType::class,
        'MajorLeaderType' => LeaderType::class,
        'MinorCultureType' => CultureType::class,
        'MinorLeaderType' => LeaderType::class,
        'ResourceManufacturedType' => ResourceType::class,
        'ResourceNaturalType' => ResourceType::class,
        'ResourceStrategicType' => ResourceType::class,
    ];

    protected static function rules(): array
    {
        return [
            'name' => ['string', 'required'],
            'description' => ['string', 'sometimes'],
            'category' => ['string', 'sometimes'],

            'actions' => ['array', 'sometimes'],
            'actions.*' => ['string', 'required'],

            'gains' => ['array', 'sometimes'],
            'gains.*.type' => ['string', 'required'],
            'gains.*.name' => ['string', 'required'],
            'gains.*.value' => ['numeric', 'sometimes'],

            'names.*.name' => ['string', 'required'],
            'names.*.platform' => ['string', 'required'],

            'requires' => ['array', 'sometimes'],

            'requires.*.category' => ['string', 'required_without:requires.*.type'],
            'requires.*.type' => ['string', 'required_without:requires.*.category'],
            'requires.*.name' => ['string', 'required_unless:requires.*.type,any'],

            'requires.*.items' => ['array', 'min:1', 'required_if:requires.*.type,any'],
            'requires.*.items.*.category' => ['string', 'required_without:requires.*.items.*.type'],
            'requires.*.items.*.type' => ['string', 'required_without:requires.*.items.*.category'],
            'requires.*.items.*.name' => ['string', 'required'],

            'specials' => ['array', 'sometimes'],
            'specials.*' => ['string', 'required'],

            'upgradesFrom' => ['array', 'sometimes'],
            'upgradesFrom.*' => ['string', 'required'],

            'yields' => ['array', 'sometimes'],
            'yields.*.name' => ['string', 'required'],
            'yields.*.value' => ['numeric', 'required'],
            'yields.*.type' => ['in:lump,percent,set', 'sometimes'],
            'yields.*.for' => ['array', 'sometimes'],
            'yields.*.for.*.category' => ['string', 'required_without:yields.*.for.*.type'],
            'yields.*.for.*.type' => ['string', 'required_without:yields.*.for.*.category'],
            'yields.*.for.*.name' => ['string', 'required'],
            'yields.*.vs' => ['array', 'sometimes'],
            'yields.*.vs.*.category' => ['string', 'required_without:yields.*.vs.*.type'],
            'yields.*.vs.*.type' => ['string', 'required_without:yields.*.vs.*.category'],
            'yields.*.vs.*.name' => ['string', 'required'],
        ];
    }

    public static function loadTypes(): void
    {
        $files = glob(resource_path('GameData/*.json'));
        $errors = [];
        foreach ($files as $file) {
            static::loadType($file, $errors);
        }
        if ($errors) {
            throw new ErrorListException($errors);
        }

        static::output("Build Relations");
        static::tryTo(fn () => ObjectStore::buildRelations(), $errors, 'buildRelations:');

        if ($errors) {
            throw new ErrorListException($errors);
        }

        static::output("Fill ObjectRef details");
        static::tryTo(fn () => ObjectRef::fillDetails(), $errors, 'fillRefDetails:');

        if ($errors) {
            throw new ErrorListException($errors);
        }
    }

    public static function loadType(string $filePath, array &$errors): void
    {
        $data = json_decode(file_get_contents($filePath), true);
        if (! $data) {
            $errors[] = "Invalid json in $filePath";
            return;
        }

        $type = pathinfo($filePath, PATHINFO_FILENAME);
        static::output("Loading $type");

        /** @var string|TypeObject $class */
        $class = static::$classes[$type] ?? null;
        if (!$class) {
            foreach (['Simple', 'Complex', 'Categorized', 'Upgrading'] as $folder) {
                $class = "App\GameClasses\Types\\$folder\\$type";
                if (class_exists($class)) {
                    break;
                } else {
                    $class = null;
                }
            }
        }
        if (! $class) {
            $errors[] = "$filePath: Type $type does not exist";
            return;
        }

        foreach ($data as $i => $item) {
            $id = $item['name'] ?? $i;

            static::tryTo(
                function () use ($item, $class) {
                    Validator::make($item, static::rules())->validate();
                    ObjectStore::set($class::fromArray($item));
                },
                $errors,
                "{$type}[$id] fromArray:"
            );
        }
    }

    public static function loadSave(string $filePath): void
    {
        $save = json_decode(file_get_contents($filePath), true);
        ObjectStore::$world = World::fromArray($save['world']);

        foreach ($save['objects'] as $items) {
            foreach ($items as $item) {
                /** @var string|WithTypeObject $class */
                $class = "App\\GameClasses\\Objects\\{$item['class']}";
                ObjectStore::set($class::fromArray($item));
            }
        }
        ObjectRef::fillDetails();
    }

    protected static function output(string $message): void
    {
        if ($output = static::$output) {
            $output($message);
        }
    }
}
